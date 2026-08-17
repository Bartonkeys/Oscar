using AutoMapper;
using BartonKeys.Extensions;
using BartonKeys.Functional;
using EFCore.BulkExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Registration.Contracts;
using System.Data;
using System.Linq.Dynamic.Core;

namespace Oscar.Infrastructure.Features.Registration.Services;

public abstract class RegistrationService<T> : IRegistrationService<T>
{
    protected IMapper _mapper;
    protected Client _client { get; set; }
    public Core.Entities.Works _works { get; set; }
    protected Core.Entities.Society _society { get; set; }
    private readonly ILogger<RegistrationService<T>> _logger;
    protected readonly OscarContext _oscarContext;
    protected readonly IMediator _mediator;
    protected readonly IServiceScopeFactory _serviceScopeFactory;
    protected int ClientId;
    protected RegistrationBatch RegistrationBatch;
    private List<Core.Entities.Catalogue> _clientCatalogues;

    public RegistrationService(OscarContext oscarContext, IMapper mapper,
        ILogger<RegistrationService<T>> logger, IMediator mediator, IServiceScopeFactory serviceScopeFactory)
    {
        _mapper = mapper;
        _logger = logger;
        _oscarContext = oscarContext;
        _mediator = mediator;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public abstract Task<Result<T>> Create(RegistrationBatch registrationBatch, int clientId);

    protected virtual async Task<Result<List<Core.Entities.Registration>>> Register(bool includeEpisodes = true)
    {
        var maybeSociety = await GetSociety(RegistrationBatch.SocietyId);

        if (!maybeSociety.HasValue)
            return Result.Fail<List<Core.Entities.Registration>>(RegistrationError.SocietyNotFound);

        _society = maybeSociety!.Value;

        var registrationsResult = await GetClientRegistrations(includeEpisodes);
        if (registrationsResult.IsFailure)
            return Result.Fail<List<Core.Entities.Registration>>(registrationsResult.Error);

        var registrations = registrationsResult.Value.ToList();

        if (UserDefinedRegistrations())
            await UpdateToRegisterStatus();
        else
            await _oscarContext.BulkInsertAsync(registrations);

        return Result.Ok(registrations);
    }

    private async Task UpdateToRegisterStatus()
    {
        var userDefinedRegistrations = _oscarContext.Registrations.Where(r => r.RegistrationBatch!.Id == RegistrationBatch.Id);
        foreach (var userDefinedRegistration in userDefinedRegistrations)
        {
            userDefinedRegistration.RegisterStatus = RegistrationBatch.DoNotRegister == true ? RegisterStatus.Unregistered : RegisterStatus.Registered;
        }

        await _oscarContext.SaveChangesAsync();
    }

    private async Task<Result<List<Core.Entities.Registration>>> GetClientRegistrations(bool includeEpisodes)
    {
        var maybeClient = await GetClientAndRights(ClientId);
        if (!maybeClient.HasValue)
            return Result.Fail<List<Core.Entities.Registration>>(RegistrationError.ClientNotFound);

        _client = maybeClient.Value;

        if (!await IsClientRightsValid(maybeClient!.Value))
            return Result.Fail<List<Core.Entities.Registration>>(RegistrationError.InvalidClientRights);

        if (!IsValidClientStatus(maybeClient!.Value))
            return Result.Fail<List<Core.Entities.Registration>>(RegistrationError.InvalidClientStatus);

        if (!IsClientLinkedToSociety(maybeClient.Value, _society))
            return Result.Fail<List<Core.Entities.Registration>>(RegistrationError.ClientNotLinkedToSociety);

        if (!IsValidClientRightsForSocietyTerritory(maybeClient.Value, _society))
            return Result.Fail<List<Core.Entities.Registration>>(RegistrationError
                .InValidClientRightsForSocietyTerritory);

        List<Core.Entities.Works> works = new List<Core.Entities.Works>();

        if (UserDefinedRegistrations())
        {
            _clientCatalogues = GetClientCataloguesWithRightsNoWorks(_client.Id).ToList();
            var userDefinedRegistrations = GetUserDefinedRegstrations();

            works = userDefinedRegistrations
                .Select(w => w.Works)
                .Where(w => WorkValid(w, RegistrationBatch.IncludePreviouslyRegistered)).ToList()!;
        }
        else
        {
            _clientCatalogues = GetClientCataloguesWithRightsAndWorks(_client.Id).ToList();

            var toBeRegisteredWorks = GetClientCatalogueWorks(_client.Id, RegistrationBatch.CatalogueId, RegistrationBatch.SocietyId,
                RegistrationBatch.IncludePreviouslyRegistered).ToList();

            var standAloneWorkIds = toBeRegisteredWorks
                .Where(x => x.Discriminator == "StandAlone")
                .Select(x => x.WorksId).ToList();

            var standAlones = new List<Core.Entities.StandAlone>();

            if (standAloneWorkIds.Any())
                standAlones = GetStandAlones(standAloneWorkIds).ToList()
                    ?.Where(w => WorkValid(w, RegistrationBatch.IncludePreviouslyRegistered)).ToList();

            var seriesCollection = new List<Core.Entities.Series>();
            var seasons = new List<Core.Entities.Season>();
            var episodes = new List<Core.Entities.Episode>();

            var seriesWorkIds = toBeRegisteredWorks
                .Where(x => x.Discriminator == "Series")
                .Select(x => x.WorksId).ToList();

            if (seriesWorkIds.Any())
                seriesCollection = GetSeries(seriesWorkIds).ToList()
                    ?.Where(w => WorkValid(w, RegistrationBatch.IncludePreviouslyRegistered)).ToList();

            if (IsRegisterSeasonsAndEpisodes(_society))
            {
                var seasonsWorkIds = toBeRegisteredWorks
                    .Where(x => x.Discriminator == "Season")
                    .Select(x => x.WorksId).ToList();

                if (seasonsWorkIds.Any())
                    seasons = GetSeasons(seasonsWorkIds).ToList()
                        ?.Where(w => WorkValid(w, RegistrationBatch.IncludePreviouslyRegistered)).ToList();

                if (includeEpisodes)
                {
                    var episodeWorkIds = toBeRegisteredWorks
                        .Where(x => x.Discriminator == "Episode")
                        .Select(x => x.WorksId).ToList();

                    if (episodeWorkIds.Any())
                        episodes = GetEpisodes(episodeWorkIds).ToList()
                            ?.Where(w => WorkValid(w, RegistrationBatch.IncludePreviouslyRegistered)).ToList();
                }


                //Add missing parents for each level for SreenRights society
                if (_society.Name == "SCREENRIGHTS" || _society.Name == "AGICOA")
                {
                    var eipsodesParentsIds = episodes?.Select(ep => ep.SeasonId).Distinct().ToList();
                    if (episodes != null && eipsodesParentsIds!.Any())
                    {
                        var seasonIds = eipsodesParentsIds.Where(id => id.HasValue).Select(id => id.Value).ToList();
                        var seasonsOfEpisodes = GetSeasons(seasonIds).ToList();
                        foreach (var season in seasonsOfEpisodes)
                        {
                            if (!seasons.Where(s => s.Id == season.Id)!.Any())
                                seasons.Add(season);
                        }

                        var seasonsParentsIds = seasonsOfEpisodes?.Select(season => season.SeriesId).Distinct().ToList();
                        if (seasonsParentsIds != null && seasonsParentsIds!.Any())
                        {
                            var seriesIds = seasonsParentsIds.Where(id => id.HasValue).Select(id => id.Value).ToList();

                            var seriesOfSeasons = GetSeries(seriesIds).ToList();
                            foreach (var series in seriesOfSeasons)
                            {
                                if (!seriesCollection.Where(s => s.Id == series.Id)!.Any())
                                {
                                    seriesCollection.Add(series);
                                }
                            }
                        }
                    }

                    //There could be some seasons that don't have any episodes yet and their parent series needs to be added as well
                    var seasonsWithoutEpisodesParentsIds = seasons?.Select(season => season.SeriesId).Distinct().ToList();
                    if (seasonsWithoutEpisodesParentsIds != null && seasonsWithoutEpisodesParentsIds!.Any())
                    {
                        var seriesIds = seasonsWithoutEpisodesParentsIds.Where(id => id.HasValue).Select(id => id.Value).ToList();

                        var seriesOfSeasons = GetSeries(seriesIds).ToList();
                        foreach (var series in seriesOfSeasons)
                        {
                            if (!seriesCollection.Where(s => s.Id == series.Id)!.Any())
                            {
                                seriesCollection.Add(series);
                            }
                        }
                    }
                }
            }

            if (standAlones != null && standAlones.Any())
                works.AddRange(standAlones);
            if (seriesCollection != null && seriesCollection.Any())
                works.AddRange(seriesCollection);
            if (seasons != null && seasons.Any())
                works.AddRange(seasons);
            if (episodes != null && episodes.Any())
                works.AddRange(episodes);
        }

        return !works.Any() ? Result.Fail<List<Core.Entities.Registration>>(RegistrationError.NoWorks)
            : Result.Ok(RegisterWorks(works, RegistrationBatch, maybeClient.Value).ToList());
    }

    private List<Core.Entities.Registration> GetUserDefinedRegstrations()
    {
        return _oscarContext.Registrations
            .AsNoTracking()
            .Include(w => w.Works).ThenInclude(c => c.Catalogues).ThenInclude(c => c.Client)
            .Include(w => w.Works).ThenInclude(r => r.Rights)!.ThenInclude(t => t.Countries)
            .Include(w => w.Works).ThenInclude(r => r.Rights)!.ThenInclude(t => t.Type)
            .Include(w => w.Works).ThenInclude(r => r.Mandates).ThenInclude(t => t.MandateType)
            .Include(w => w.Works).ThenInclude(r => r.Countries)
            .AsSplitQuery()
            .Where(r => r.RegistrationBatch!.Id == RegistrationBatch.Id && r.RegisterStatus == RegisterStatus.UserSelected).ToList();
    }

    private bool WorkValid(Core.Entities.Works works, bool includePreviouslyRegistered)
    {
        if (!CheckWorkReRegisteredForSocietyThenRemove(works) && !includePreviouslyRegistered && IsWorksPreviouslyRegisteredBySociety(works, _society)) return false;

        if (!IsValidWorkRightsForSocietyTerritory(works, _society)) return false;

        if (!IsSocietyRightsClaimableOnWork(works, _society)) return false;

        if (works.Mandates.Count == 0) return false;

        return true;
    }

    private bool CheckWorkReRegisteredForSocietyThenRemove(Core.Entities.Works works)
    {
        var rereg = _oscarContext.ReRegistrations.FirstOrDefault(r => r.Works.Id == works.Id);
        if (rereg != null)
        {
            //The re-register is removed only if the batch is set with false in DoNotRegister flag
            if (RegistrationBatch.DoNotRegister == false)
                _oscarContext.ReRegistrations.Remove(rereg);
            return true;
        }
        else
            return false;
    }

    private bool UserDefinedRegistrations() => _oscarContext.Registrations.Any(r =>
        r.RegistrationBatch!.Id == RegistrationBatch.Id && r.RegisterStatus == RegisterStatus.UserSelected);

    private async Task<Maybe<Core.Entities.Society>> GetSociety(int? societyId)
    {
        return (await _oscarContext
                .Societies
                .AsNoTracking()
                .Include(r => r.SocietyRights)!.ThenInclude(t => t.RightsType)
                .Include(r => r.SocietyRights)!.ThenInclude(t => t.Country)
                .AsSplitQuery()
                .SingleOrDefaultAsync(s => s.Id == societyId))
            .ToMaybe()!;
    }

    private async Task<Maybe<Client?>> GetClientAndRights(int? clientId)
    {
        var client = await _oscarContext
            .Clients
            .AsNoTracking()
            .Include(s => s.Societies)
            .SingleAsync(s => s.Id == clientId);

        client.Rights = _oscarContext
            .Rights
            .AsNoTracking()
            .Include(t => t.Type)
            .Include(t => t.Countries)
            .Where(r => r.Client.Id == clientId && r.Work == null).ToList();

        return client.ToMaybe()!;
    }

    public List<ClientCatalogueSocietyWork> GetClientCatalogueWorks(int? clientId, int? catalogueId, int? societyId, bool includePreviouslyRegistered)
    {
        var result = _oscarContext.ClientCatalogueSocietyWorks
            .FromSqlInterpolated($"[dbo].[sp_GetClientCatalogueSocietyWorks] {clientId}, {catalogueId}, {societyId}, {includePreviouslyRegistered}")
            .ToList();

        return result;
    }

    private IQueryable<Core.Entities.Catalogue> GetClientCataloguesWithRightsAndWorks(int? clientId)
    {
        return _oscarContext
            .Catalogues
            .AsNoTracking()
            .Include(w => w.Client)
            .Include(r => r.Rights)!.ThenInclude(t => t.Type)
            .Include(r => r.Rights)!.ThenInclude(t => t.Work)
            .Include(w => w.Rights)!.ThenInclude(c => c.Countries)
            .Include(w => w.Rights)!.ThenInclude(c => c.LanguageRights).ThenInclude(l => l.Language)
            .Include(w => w.Rights)!.ThenInclude(c => c.ChannelRights).ThenInclude(l => l.Channel)
            .Include(w => w.Works).ThenInclude(r => r.Rights)!.ThenInclude(t => t.Type)
            .Include(w => w.Works).ThenInclude(r => r.Rights)!.ThenInclude(t => t.Countries)
            .Include(w => w.Works).ThenInclude(r => r.Mandates).ThenInclude(t => t.MandateType)
            .Include(w => w.Works).ThenInclude(r => r.Countries)
            .AsSplitQuery()
            .Where(c =>
                c.Client.Id == clientId && c.Works.Any(w => (w.WorksStatus == Core.Enums.WorksStatus.Active || w.WorksStatus == Core.Enums.WorksStatus.InConflict) && w.Discriminator == "Series" || w.Discriminator == "StandAlone"));
    }

    private IQueryable<Core.Entities.Catalogue> GetClientCataloguesWithRightsNoWorks(int? clientId)
    {
        return _oscarContext
            .Catalogues
            .AsNoTracking()
            .Include(w => w.Client)
            .Include(r => r.Rights)!.ThenInclude(t => t.Type)
            .Include(r => r.Rights)!.ThenInclude(t => t.Work)
            .Include(w => w.Rights)!.ThenInclude(c => c.Countries)
            .Include(w => w.Rights)!.ThenInclude(c => c.LanguageRights).ThenInclude(l => l.Language)
            .Include(w => w.Rights)!.ThenInclude(c => c.ChannelRights).ThenInclude(l => l.Channel)
            .AsSplitQuery()
            .Where(c =>
                c.Client.Id == clientId && c.Works.Any(w => w.Discriminator == "Series" || w.Discriminator == "StandAlone"));
    }

    private IQueryable<Core.Entities.StandAlone> GetStandAlones(IList<int> standAloneWorksIds)
    {
        var standAlones = _oscarContext
            .StandAlones
            .AsNoTracking()
            .Include(r => r.Rights)!.ThenInclude(t => t.Catalogue)
            .Include(r => r.Rights)!.ThenInclude(t => t.Type)
            .Include(r => r.Rights)!.ThenInclude(t => t.Countries)
            .Include(r => r.Mandates).ThenInclude(t => t.MandateType)
            .Include(w => w.Catalogues).ThenInclude(c => c.Client)
            .Include(r => r.Countries)
            .AsSplitQuery()
            .Where(w => standAloneWorksIds.Any(worksId => worksId == w.Id));

        //Some old works migrated from Felix had multiple catalogues linked to rights even though works had only 1 catalogue associated
        //This seems to be potential data issue that we inherited form Felix, so filter and reset rights using catalogue attached to works
        foreach (var standAlone in standAlones)
        {
            standAlone.Rights = standAlone?.Rights?.Where(r => r?.Catalogue?.Id == standAlone?.Catalogues?.FirstOrDefault()?.Id).ToList();
        }

        return standAlones;
    }
    private IQueryable<Core.Entities.Series> GetSeries(IList<int> seriesWorksIds)
    {
        var seriesCollection = _oscarContext
            .Series
            .AsNoTracking()
            .Include(r => r.Rights)!.ThenInclude(t => t.Catalogue)
            .Include(r => r.Rights)!.ThenInclude(t => t.Type)
            .Include(r => r.Rights)!.ThenInclude(t => t.Countries)
            .Include(r => r.Mandates).ThenInclude(t => t.MandateType)
            .Include(w => w.Catalogues).ThenInclude(c => c.Client)
            .Include(r => r.Countries)
            .AsSplitQuery()
            .Where(w => seriesWorksIds.Any(worksId => worksId == w.Id));

        //Some old works migrated from Felix had multiple catalogues linked to rights even though works had only 1 catalogue associated
        //This seems to be potential data issue that we inherited form Felix, so filter and reset rights using catalogue attached to works
        foreach (var series in seriesCollection)
        {
            series.Rights = series?.Rights?.Where(r => r?.Catalogue?.Id == series?.Catalogues?.FirstOrDefault()?.Id).ToList();
        }

        return seriesCollection;
    }

    private IQueryable<Core.Entities.Season> GetSeasons(IList<int> seasonClientCatalogueWorksIds)
    {
        var seasons = _oscarContext
            .Seasons
            .AsNoTracking()
            .Include(r => r.Rights)!.ThenInclude(t => t.Catalogue)
            .Include(r => r.Rights)!.ThenInclude(t => t.Type)
            .Include(r => r.Rights)!.ThenInclude(t => t.Countries)
            .Include(r => r.Mandates).ThenInclude(t => t.MandateType)
            .Include(w => w.Catalogues).ThenInclude(c => c.Client)
            .Include(r => r.Countries)
            .AsSplitQuery()
            .Where(w => seasonClientCatalogueWorksIds.Any(worksId => worksId == w.Id));

        //Some old works migrated from Felix had multiple catalogues linked to rights even though works had only 1 catalogue associated
        //This seems to be potential data issue that we inherited form Felix, so filter and reset rights using catalogue attached to works
        foreach (var season in seasons)
        {
            season.Rights = season?.Rights?.Where(r => r?.Catalogue?.Id == season?.Catalogues?.FirstOrDefault()?.Id).ToList();
        }

        return seasons;
    }

    private IQueryable<Core.Entities.Episode> GetEpisodes(IList<int> episodeWorksIds)
    {
        var episodes = _oscarContext
            .Episodes
            .AsNoTracking()
            .Include(r => r.Rights)!.ThenInclude(t => t.Catalogue)
            .Include(r => r.Rights)!.ThenInclude(t => t.Type)
            .Include(r => r.Rights)!.ThenInclude(t => t.Countries)
            .Include(r => r.Mandates).ThenInclude(t => t.MandateType)
            .Include(w => w.Catalogues).ThenInclude(c => c.Client)
            .Include(r => r.Countries)
            .AsSplitQuery()
            .Where(w => episodeWorksIds.Any(worksId => worksId == w.Id));

        //Some old works migrated from Felix had multiple catalogues linked to rights even though works had only 1 catalogue associated
        //This seems to be potential data issue that we inherited form Felix, so filter and reset rights using catalogue attached to works
        foreach (var episode in episodes)
        {
            episode.Rights = episode?.Rights?.Where(r => r?.Catalogue?.Id == episode?.Catalogues?.FirstOrDefault()?.Id).ToList();
        }

        return episodes;

    }

    private IEnumerable<Core.Entities.Registration> RegisterWorks(IEnumerable<Core.Entities.Works> works,
        RegistrationBatch registrationBatch, Client client)
    {
        return works.Select(work => new Core.Entities.Registration
        {
            RegistrationBatch = registrationBatch,
            RegisterStatus = registrationBatch.DoNotRegister == true ? RegisterStatus.Unregistered : RegisterStatus.Registered,
            Client = client,
            Works = work,
            Society = _society,
            RegisterType = RegisterType.Zero,
            ModifiedBy = "RegistrationFunction",
            DateRegistered = registrationBatch.DateRegistered
        });
    }

    internal bool IsValidClientStatus(Client client)
    {
        //If the client has status NACC or TERMINATED (NFC) then works may not be registered
        //The status of the owning client must be Active(Consolidated, In Term and Lapsed, are all good)
        if (client != null)
            return client.Status == Status.Active_Consolidated ||
                   client.Status == Status.Active_In_Term ||
                   client.Status == Status.Active_Lapsed;
        return false;
    }

    protected abstract Task<bool> IsClientRightsValid(Client client);

    protected virtual bool IsClientLinkedToSociety(Client client, Core.Entities.Society society)
    {
        return client != null && client.Societies != null && client.Societies.Any(sr => sr.Id == society.Id);
    }

    protected virtual bool IsValidWorkRightsForSocietyTerritory(Core.Entities.Works works, Core.Entities.Society society)
    {
        if ((works.Rights == null) | (works.Rights!.Count == 0))
        {
            works.Rights = InheritWorksRightsFromParent(works);
        }

        return CheckSocietyRightsAndTerritory(society, works.Rights);
    }

    protected ICollection<Right> InheritWorksRightsFromParent(Core.Entities.Works works)
    {
        var results = new List<Right>();
        var discriminator = Enum.Parse<Discriminator>(works.Discriminator!);
        switch (discriminator)
        {
            case Discriminator.StandAlone:
            case Discriminator.Series:
                results = InheritCatalogueRights(works);
                break;
            case Discriminator.Season:
                results = InheritSeriesRights(discriminator, works);
                if (!results.Any())
                {
                    results = InheritCatalogueRights(works);
                }

                break;
            case Discriminator.Episode:
                results = InheritSeasonRights(works);
                if (!results.Any()) results = InheritSeriesRights(discriminator, works);
                if (!results.Any())
                {
                    results = InheritCatalogueRights(works);
                }

                break;
        }

        return results;
    }

    private List<Right> UpdateRightsStartDate(Core.Entities.Works works, List<Right> results)
    {
        if (results != null)
        {
            foreach (var right in results.Where(right => (works?.ProductionYear != null) && (works?.ProductionYear is > 0)))
            {
                right.StartOfRight = new DateTime(works.ProductionYear!.Value, 1, 1);
                right.StartOfValidity = new DateTime(works.ProductionYear!.Value, 1, 1);
            }
        }
        return results;
    }

    private List<Right> UpdateRightsClient(Core.Entities.Works works, List<Right> rightsResult)
    {
        if (rightsResult != null)
        {
            var worksClient = works!.Catalogues!.FirstOrDefault()?.Client;

            if (worksClient != null)
            {
                //inherited rights don't get saved in rights table and hence need to update them with client information below that is needed in populating client details in registrations
                foreach (var right in rightsResult)
                {
                    right.Client = new Client();
                    right.Client.Id = worksClient.Id;
                    right.Client.ClientReference = worksClient.ClientReference;
                    right.Client.ClientName = worksClient.ClientName;
                    right.Client.IMaestroClientCode = worksClient.IMaestroClientCode;
                }
            }
        }

        return rightsResult;
    }

    private List<Right> InheritCatalogueRights(Core.Entities.Works works)
    {
        var catalogueId = works.Catalogues.First(cat => cat.Client.Id == ClientId).Id;
        var rightsResult = _clientCatalogues.First(c => c.Id == catalogueId).Rights!.Where(r => r.Work == null).ToList();
        rightsResult = UpdateRightsStartDate(works, rightsResult);
        rightsResult = UpdateRightsClient(works, rightsResult);
        return rightsResult;
    }

    private List<Right> InheritSeriesRights(Discriminator discriminator, Core.Entities.Works works)
    {
        int? parentWorksId = null;
        switch (discriminator)
        {
            case Discriminator.Season:
                var seasonResult = _oscarContext.Seasons.Single(s => s.Id == works.Id);
                parentWorksId = seasonResult.SeriesId;
                break;
            case Discriminator.Episode:
                var episodeResult = _oscarContext.Episodes.Single(s => s.Id == works.Id);
                parentWorksId = episodeResult.SeriesId;
                break;
        }

        if (parentWorksId == null) return new List<Right>();

        var rightsResult = GetRightsForWorks(parentWorksId);

        rightsResult = UpdateRightsStartDate(works, rightsResult);
        rightsResult = UpdateRightsClient(works, rightsResult);

        return rightsResult;
    }

    private List<Right> InheritSeasonRights(Core.Entities.Works works)
    {
        int? parentWorksId = null;
        var episodeResult = _oscarContext.Episodes.Single(s => s.Id == works.Id);
        parentWorksId = episodeResult!.SeasonId;

        if (parentWorksId == null) return new List<Right>();

        var rightsResult = GetRightsForWorks(parentWorksId);

        rightsResult = UpdateRightsStartDate(works, rightsResult);
        rightsResult = UpdateRightsClient(works, rightsResult);

        return rightsResult;
    }

    private List<Right> GetRightsForWorks(int? parentWorksId)
    {
        var worksCatalogueId = _oscarContext
            .Works
            .Include(w => w.Catalogues)
            ?.Where(w => w.Id == parentWorksId).FirstOrDefault()
            ?.Catalogues?.FirstOrDefault()?.Id;

        return _oscarContext
            .Rights
            .AsNoTracking()
            .Include(r => r.Type)
            .Include(cr => cr.ChannelRights).ThenInclude(t => t.Channel)
            .Include(cr => cr.ChannelRights).ThenInclude(t => t.CountryRights)
            .Include(lr => lr.LanguageRights).ThenInclude(l => l.Language)
            .Include(c => c.Countries)
            .AsSplitQuery()
            .Where(r => r.Work != null && r.Work.Id == parentWorksId && r.Catalogue != null && r.Catalogue.Id == worksCatalogueId)
            .ToList();
    }

    internal virtual bool IsValidClientRightsForSocietyTerritory(Client client, Core.Entities.Society society)
    {
        return CheckSocietyRightsAndTerritory(society, client.Rights);
    }

    protected virtual bool IsSocietyRightsClaimableOnWork(Core.Entities.Works works, Core.Entities.Society society)
    {
        return true;
    }

    internal bool IsClientNotTerminatedBeforeEndOfRegistrationYear(Client client, DateTime? dateRegistered)
    {
        //If the registration is for a specific year and the client was TERMINATED before the end of that year then works may not be registered
        return true;
    }

    internal bool IsRegisterSeasonsAndEpisodes(Core.Entities.Society society)
    {
        //TODO: add field to Society entity to determine if that society registers Seasons & Episodes (currently just in EGEDA and MPLC)

        if (society.Name.Equals("MPLC"))
            return false;

        return !society.Name.Equals("EGEDA");
    }

    internal bool IsWorksPreviouslyRegisteredBySociety(Core.Entities.Works works, Core.Entities.Society society)
    {
        //Work must be marked as Not Previously Registered for the society (may be overridden by certain run time selections)
        var result = _oscarContext.Registrations.Any(x =>
            x.Works != null && x.Works.Id == works.Id &&
            x.Society != null && x.Society.Id == society.Id &&
            x.RegisterStatus == RegisterStatus.Registered);

        return result;
    }

    /// <summary>
    /// Client References were not getting added after migration until the fix was done and hence some of the works got created with missing client references and hence adding them below.
    /// This is needed only for CMC registrations
    /// </summary>
    /// <returns></returns>
    protected async Task AddRegisteredWorksMissingClientReferences()
    {
        var registrations = await _oscarContext
            .Registrations
            .Include(r => r.Works).ThenInclude(w => w.ClientReferences)
            .Include(r => r.Works).ThenInclude(c => c.Catalogues)!.ThenInclude(c => c.Client)
            .Where(r => r.RegistrationBatch.Id == RegistrationBatch.Id)
            .ToListAsync();

        foreach (var registration in registrations)
        {
            if (registration.Works.ClientReferences == null || registration.Works.ClientReferences?.Count() == 0)
            {
                registration.Works.ClientReferences = new List<ClientReference>
                {
                    new ClientReference
                    {
                        Works = registration.Works,
                        Client = registration.Works.Catalogues.First().Client,
                        Catalogue = registration.Works.Catalogues.First()
                    }
                };
            }
            else if (registration.Works.ClientReferences.FirstOrDefault()?.Client == null) //update if client is null
            {
                registration.Works.ClientReferences.First().Client = registration.Works.Catalogues.First().Client;
                registration.Works.ClientReferences.First().Catalogue = registration.Works.Catalogues.First();
            }
        }
        await _oscarContext.SaveChangesAsync();
    }

    protected async Task<List<Core.Entities.Registration>> GetRegistrations()
    {
        var registrations = await _oscarContext
            .Registrations
            .AsNoTracking()
            .Include(r => r.Works).ThenInclude(w => w.Titles)
            .Include(r => r.Works).ThenInclude(w => w.WorksType)
            .Include(r => r.Works).ThenInclude(w => w.Genre)
            .Include(r => r.Works).ThenInclude(w => w.Languages)
            .Include(r => r.Works).ThenInclude(w => w.Directors)
            .Include(r => r.Works).ThenInclude(w => w.Actors)
            .Include(r => r.Works).ThenInclude(w => w.Producers)
            .Include(r => r.Works).ThenInclude(w => w.Distributors)
            .Include(r => r.Works).ThenInclude(w => w.ScriptWriters)
            .Include(r => r.Works).ThenInclude(w => w.ScreenWriters)
            .Include(r => r.Works).ThenInclude(w => w.Countries)
            .Include(r => r.Works).ThenInclude(w => w.Companies)
            .Include(r => r.Works).ThenInclude(w => w.Clients!.Where(cr => cr!.Id == ClientId))
            .Include(r => r.Works).ThenInclude(w => w.ClientReferences!.Where(cr => cr.Client!.Id == ClientId)).ThenInclude(c => c.Client)
            .Include(r => r.Works).ThenInclude(w => w.Rights)!.ThenInclude(c => c.Catalogue)
            .Include(r => r.Works).ThenInclude(w => w.Rights)!.ThenInclude(c => c.Client)
            .Include(r => r.Works).ThenInclude(w => w.Rights)!.ThenInclude(c => c.Type)
            .Include(r => r.Works).ThenInclude(w => w.Rights)!.ThenInclude(c => c.Countries)
            .Include(r => r.Works).ThenInclude(w => w.Rights)!.ThenInclude(c => c.LanguageRights).ThenInclude(l => l.Language)
            .Include(r => r.Works).ThenInclude(w => w.Rights)!.ThenInclude(c => c.ChannelRights).ThenInclude(l => l.Channel)
            .Include(r => r.Works).ThenInclude(w => w.Mandates).ThenInclude(m => m.MandateType)
            .Include(r => r.Works).ThenInclude(c => c.Catalogues)!.ThenInclude(c => c.Client)
            .AsSplitQuery()
            .Where(r => r.RegistrationBatch.Id == RegistrationBatch.Id && r.Client.Id == ClientId)
            .ToListAsync();

        //Not deleting to keep track of the registrations data
        //if (RegistrationBatch.DoNotRegister == true)
        //{
        //    await _oscarContext.BulkDeleteAsync(registrations);
        //}

        //Some old works migrated from Felix had multiple catalogues linked to rights even though works had only 1 catalogue associated
        //This seems to be potential data issue that we inherited form Felix, so filter and reset rights using catalogue attached to works
        foreach (var registration in registrations)
        {
            registration.Works.Rights = registration.Works?.Rights?.Where(r => r?.Catalogue?.Id == registration.Works?.Catalogues?.FirstOrDefault()?.Id).ToList();
        }

        return registrations;
    }

    protected virtual bool CheckSocietyRightsAndTerritory(Core.Entities.Society society, ICollection<Right>? rights)
    {
        if ((rights == null) | (rights!.Count == 0))
            return false;

        foreach (var societyRight in society.SocietyRights)
        {
            if (rights.All(r => r.Type.Name != societyRight.RightsType.Name)) continue;

            var worksRights = rights.Where(r => r.Type.Name == societyRight.RightsType.Name && r.Percentage is > 0);

            if (societyRight.Country.Name == "WORLD" && worksRights.Any())
                return true;

            if (worksRights.Any(worksRight => worksRight.Countries.Any(c => c.Code == societyRight.Country.Code)))
                return true;

            if (worksRights.Any(worksRight => worksRight.Countries.Any(c => c.Name == "WORLD")))
                return true;

        }

        return false;
    }

    protected static string SanitizeAsFileName(string originalName)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        string sanitizedFileName = new string(originalName.Select(c => invalidChars.Contains(c) ? '-' : c).ToArray());
        return sanitizedFileName;
    }

}
