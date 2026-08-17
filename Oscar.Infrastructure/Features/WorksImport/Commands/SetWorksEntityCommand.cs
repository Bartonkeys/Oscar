using AutoMapper;
using Azure.Core;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Index.Quadtree;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Rights.Queries;

namespace Oscar.Infrastructure.Features.WorksImport.Commands
{
    public class SetWorksEntityCommand: IRequest<Result<Core.Entities.Works>>
    {
        public WorksImportRequest worksImportRequest { get; set; } 
        public Core.Entities.WorksImport worksImport { get; set; } 
        public Client client { get; set; }
        public Oscar.Core.Entities.Catalogue? catalogue { get; set; }
        public TitleType titleType { get; set; } = TitleType.Main;
        public bool isAgicoa { get; set; } = true; //todo: pmalik - remove default value and set it from UI when ready
    }

    public class SetWorksEntityCommandHandler : AbstractBaseHandler<SetWorksEntityCommand, Core.Entities.Works>
    {
        private readonly IMediator _mediator;

        public SetWorksEntityCommandHandler(OscarContext oscarContext, IMapper mapper, 
            IValidator<SetWorksEntityCommand> validator, ILogger<SetWorksEntityCommand> logger, IMediator mediator) 
            : base(oscarContext, mapper, validator, logger)
        {
            _mediator = mediator;
        }

        protected override async Task<Result<Core.Entities.Works>> HandleRequest(SetWorksEntityCommand request, CancellationToken cancellationToken)
        {
            var works = new Core.Entities.Works
            {
                WorksStatus = Core.Enums.WorksStatus.Active,

                Titles = new List<WorksTitle> { new WorksTitle { Title = request.worksImport.Title, TitleType = request.titleType, LanguageCode = request.worksImport.TitleLanguage.ToUpper() } },

                ProductionYear = request.worksImport.ProductionYear == null ? null : int.Parse(request.worksImport.ProductionYear),

                DurationMinutes = request.worksImport.Duration == null ? null : int.Parse(request.worksImport.Duration),

                WorksImportRequest = request.worksImportRequest,

                Clients = new List<Client>() { request.client },

                Rights = await AssignRights(request.isAgicoa, request.worksImport.Id, request.client, request.catalogue)
            };

            if (request.catalogue != null)
            {
                works.Catalogues = new List<Core.Entities.Catalogue>() { request.catalogue };
            }

            if (!string.IsNullOrWhiteSpace(request.worksImport.DirectorFirstName) || !string.IsNullOrWhiteSpace(request.worksImport.DirectorLastName))
            {
                works.Directors = new List<Oscar.Core.Entities.Director>
                {
                    new() { FirstName = request.worksImport.DirectorFirstName ?? "", LastName = request.worksImport.DirectorLastName ?? "" }
                };
            }

            await SetProductionCompany(request.worksImport.ProductionCompany1, works);
            await SetProductionCompany(request.worksImport.ProductionCompany2, works);
            await SetProductionCompany(request.worksImport.ProductionCompany3, works);

            await SetProductionCountry(request.worksImport.ProductionCountry1, works);
            await SetProductionCountry(request.worksImport.ProductionCountry2, works);
            await SetProductionCountry(request.worksImport.ProductionCountry3, works);
            await SetProductionCountry(request.worksImport.ProductionCountry4, works);

            await SetActor(request.worksImport.Actor1FirstName, request.worksImport.Actor1LastName, works);
            await SetActor(request.worksImport.Actor2FirstName, request.worksImport.Actor2LastName, works);
            await SetActor(request.worksImport.Actor3FirstName, request.worksImport.Actor3LastName, works);

            if (!string.IsNullOrWhiteSpace(request.worksImport.Genre))
            {
                var genre = await OscarContext.Genres.FirstOrDefaultAsync(g => g.Name.ToLower() == request.worksImport.Genre.ToLower());
                if (genre != null)
                {
                    works.Genre = genre;
                }
            }

            if (!string.IsNullOrWhiteSpace(request.worksImport.WorkSubType))
            {
                var worksSubType = await OscarContext.WorksSubTypes.FirstOrDefaultAsync(g => g.Name.ToLower() == request.worksImport.WorkSubType.ToLower());
                if (worksSubType != null)
                {
                    works.WorksSubType = worksSubType;
                }
            }

            if (!string.IsNullOrWhiteSpace(request.worksImport.CavcoCode))
            {
                works.CavcoCode = request.worksImport.CavcoCode;
            }

            if (!string.IsNullOrWhiteSpace(request.worksImport.CrtcCode))
            {
                works.CrtcCode = request.worksImport.CrtcCode;
            }

            await SetAltTitle(request.worksImport.AKATitle1, request.worksImport.AKATitle1Language, works);
            await SetAltTitle(request.worksImport.AKATitle2, request.worksImport.AKATitle2Language, works);
            await SetAltTitle(request.worksImport.AKATitle3, request.worksImport.AKATitle3Language, works);

            if (!string.IsNullOrWhiteSpace(request.worksImport.Colour))
            {
                works.ColourFormat = request.worksImport.Colour.ToUpper();
            }
            else
            {
                works.ColourFormat = "COLOUR";
            }

            if (request.worksImport.WorksType != null)
            {
                if (request.worksImport.WorksType.ToLower() == "season")
                {
                    if (request.isAgicoa)
                        works.CompactRef = request.worksImport.SeasonNumber;
                    else
                        works.Number = int.TryParse(request.worksImport.SeasonNumber, out int result) ? result : (int?)null;

                    works.WorksType = OscarContext.WorksTypes.Single(w => w.Name == "SE");
                }
                else if (request.worksImport.WorksType.ToLower() == "episode")
                {
                    if (request.isAgicoa)
                        works.CompactRef = request.worksImport.EpisodeNumber;
                    else
                        works.Number = int.TryParse(request.worksImport.EpisodeNumber, out int result) ? result : (int?)null;

                    works.WorksType = OscarContext.WorksTypes.Single(w => w.Name == "SE");
                }
                else if (request.worksImport.WorksType.ToLower() == "stand alone")
                {
                    if (request.isAgicoa)
                        works.CompactRef = request.worksImport.SASeriesNumber;

                    var duration = Convert.ToInt32(request.worksImport.Duration);
                    if (duration > 50)
                        works.WorksType = OscarContext.WorksTypes.Single(w => w.Name == "SH");
                    else
                        works.WorksType = OscarContext.WorksTypes.Single(w => w.Name == "TF");
                }
                else if (request.worksImport.WorksType.ToLower() == "series")
                {
                    if (request.isAgicoa)
                        works.CompactRef = request.worksImport.SASeriesNumber;
                    else
                        works.Number = int.TryParse(request.worksImport.SASeriesNumber, out int result) ? result : (int?)null;

                    works.WorksType = OscarContext.WorksTypes.Single(w => w.Name == "SE");
                }
            }

            return Result.Ok(works);
        }

        private async Task<ICollection<Right>> AssignRights(bool isAgicoa, int worksImportId, Client client, Core.Entities.Catalogue? catalogue)
        {
            if (isAgicoa)
            {
                return await AssignRightsFromWorksImport(worksImportId, client, catalogue); 
            }
            else
            {
                return await AssignRightsFromClient(client, catalogue);
            }
        }

        private async Task<ICollection<Right>> AssignRightsFromClient(Client client, Core.Entities.Catalogue? catalogue)
        {
            var catalogueId = catalogue?.Id ?? (await GetDefaultCatalogueAsync(client)).Id;

            if (catalogue == null)
                catalogue = await OscarContext.Catalogues.SingleAsync(c => c.Id == catalogueId);

            var rightsResult = await _mediator.Send(new GetRightsByClientIdQuery { ClientId = client.Id, CatalogueId = catalogueId });

            var rights = rightsResult.Value.Select(rightsDto => Mapper.Map<Right>(rightsDto)).ToList();

            foreach (var right in rights)
            {
                right.Client = client;
                right.Catalogue = catalogue;
                right.Type = await OscarContext.RightsTypes.SingleAsync(t => t.Id == right.Type.Id);
                right.Countries = await OscarContext.Country.Where(c => right.Countries.Select(c => c.Id).ToList().Contains(c.Id)).ToListAsync();
                foreach (var languageRight in right.LanguageRights)
                {
                    languageRight.Language =
                        await OscarContext.Languages.SingleOrDefaultAsync(l => l.Id == languageRight.Language.Id);
                }
                foreach (var channelRight in right.ChannelRights)
                {
                    channelRight.Channel =
                        await OscarContext.Channel.SingleOrDefaultAsync(l => l.Id == channelRight.Channel.Id);
                }
            }

            return rights;
        }

        private async Task<ICollection<Right>> AssignRightsFromWorksImport(int worksImportId, Client client, Core.Entities.Catalogue? catalogue)
        {
            var catalogueId = catalogue?.Id ?? (await GetDefaultCatalogueAsync(client)).Id;
            if (catalogue == null)
                catalogue = await OscarContext.Catalogues.SingleAsync(c => c.Id == catalogueId);

            var rightsResult = await _mediator.Send(new GetRightsByWorksImportIdQuery { WorksImportId = worksImportId });

            var groupedRights = rightsResult.Value
                .GroupBy(r => new
                {
                    r.StartOfRight,
                    r.EndOfRight,
                    r.StartOfValidity,
                    r.EndOfValidity,
                    r.Percentage
                })
                .Select(g => new
                {
                    StartOfRight = g.Key.StartOfRight,
                    EndOfRight = g.Key.EndOfRight,
                    StartOfValidity = g.Key.StartOfValidity,
                    EndOfValidity = g.Key.EndOfValidity,
                    Percentage = g.Key.Percentage,
                    Countries = g.SelectMany(r => r.Countries).Distinct().ToList(),
                    LanguageRights = g.SelectMany(r => r.LanguageRights).Distinct().ToList(),
                    ChannelRights = g.SelectMany(r => r.ChannelRights).Distinct().ToList(),
                    Rights = g.ToList()
                });

            var countryCodes = groupedRights.SelectMany(gr => gr.Countries.Select(c => c.Code.ToUpper())).Distinct().ToList();
            var languageNames = groupedRights.SelectMany(gr => gr.LanguageRights.Select(lr => lr.Language.Name.ToUpper())).Distinct().ToList();
            var channelNames = groupedRights.SelectMany(gr => gr.ChannelRights.Select(cr => cr.Channel.Name.ToUpper())).Distinct().ToList();

            //using batched queries to get all below entities populated with given parameter with their respective single calls 
            //and these will be consumed in creating newRights
            var rightsTypes = await OscarContext.RightsTypes.ToDictionaryAsync(t => t.Name);
            var countries = await OscarContext.Country.Where(c => countryCodes.Contains(c.Code)).ToDictionaryAsync(c => c.Code.ToUpper());
            var languages = await OscarContext.Languages.Where(l => languageNames.Contains(l.Name)).ToDictionaryAsync(l => l.Name.ToUpper());
            var channels = await OscarContext.Channel.Where(c => channelNames.Contains(c.Name)).ToDictionaryAsync(c => c.Name.ToUpper());

            var newRights = new List<Right>();

            //Duplicate rights for each rights types
            foreach (var rightType in rightsTypes)
            {
                foreach (var groupedRight in groupedRights)
                {
                    var countriesList = new List<Core.Entities.Country>();
                    foreach (var country in groupedRight.Countries.GroupBy(c => c.Code).Select(gr => gr.First()))
                    {
                        var c = countries.First(c => c.Key == country.Code);
                        countriesList.Add(c.Value);
                    }

                    var languageRightsList = new List<Core.Entities.LanguageRights>();
                    foreach (var languageRight in groupedRight.LanguageRights.GroupBy(lr => lr.Language.Name).Select(gr => gr.First()))
                    {
                        var language = languages.First(c => c.Key == languageRight.Language.Name);
                        languageRightsList.Add(new LanguageRights { Language = language.Value });
                    }

                    var channelRightsList = new List<Core.Entities.ChannelRights>();
                    foreach (var channelRight in groupedRight.ChannelRights.GroupBy(ch => ch.Channel.Name).Select(gr => gr.First()))
                    {
                        var channel = channels.First(c => c.Key == channelRight.Channel.Name);
                        channelRightsList.Add(new ChannelRights { Channel = channel.Value });
                    }

                    var newRight = new Right
                    {
                        Client = client,
                        Catalogue = catalogue,
                        Type = rightsTypes[rightType.Value.Name],
                        StartOfRight = groupedRight.StartOfRight,
                        EndOfRight = groupedRight.EndOfRight,
                        StartOfValidity = groupedRight.StartOfValidity,
                        EndOfValidity = groupedRight.EndOfValidity,
                        Percentage = groupedRight.Percentage,
                        Countries = countriesList,
                        LanguageRights = languageRightsList,
                        ChannelRights = channelRightsList
                    };

                    newRights.Add(newRight);
                }
            }

            return newRights;
        }

        private async Task<Core.Entities.Catalogue> GetDefaultCatalogueAsync(Client client)
        {
            if (client.Catalogues != null && client.Catalogues.Any())
            {
                var catalogue = await OscarContext.Catalogues
                    .SingleOrDefaultAsync(c => c.Name == client.ClientName && client.Catalogues.Select(x => x.Id).Contains(c.Id));

                if (catalogue != null)
                {
                    return catalogue;
                }

                return await OscarContext.Catalogues.FirstOrDefaultAsync(c => client.Catalogues.Select(x => x.Id).Contains(c.Id));
            }

            return null;
        }

        private async Task SetAltTitle(string? title, string? language, Core.Entities.Works works)
        {
            if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(language))
            {
                if (works.AlternativeTitles == null) works.AlternativeTitles = new List<AlternativeTitle>();

                var languageEntity = await OscarContext.Languages.FirstOrDefaultAsync(l => l.Name.ToLower() == language.ToLower());
                if (languageEntity != null)
                {
                    works.AlternativeTitles.Add(new AlternativeTitle() { Name = title, Language = languageEntity });
                }
            }
        }

        private async Task SetActor(string? firstName, string? lastName, Core.Entities.Works works)
        {
            if (!string.IsNullOrWhiteSpace(firstName) || !string.IsNullOrWhiteSpace(lastName))
            {
                if (works.Actors == null) works.Actors = new List<Oscar.Core.Entities.Actor>();

                var existingActor = await OscarContext.Actors.FirstOrDefaultAsync(c => c.FirstName.ToLower() == (firstName ?? "").ToLower() && c.LastName == (lastName ?? "").ToLower());
                if (existingActor != null)
                {
                    works.Actors.Add(existingActor);
                }
                else
                {
                    works.Actors.Add(new Oscar.Core.Entities.Actor() { FirstName = firstName ?? "", LastName = lastName ?? "" });
                }
            }
        }

        private async Task SetProductionCompany(string? companyName, Core.Entities.Works works)
        {
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                if (works.Companies == null) works.Companies = new List<Company>();

                var existingCompany = await OscarContext.Companies.FirstOrDefaultAsync(c => c.Name == companyName);
                if (existingCompany != null)
                {
                    works.Companies.Add(existingCompany);
                }
                else
                {
                    works.Companies.Add(new Company() { Name = companyName });
                }
            }
        }

        private async Task SetProductionCountry(string? country, Core.Entities.Works works)
        {
            if (!string.IsNullOrWhiteSpace(country))
            {
                if (works.Countries == null) works.Countries = new List<Core.Entities.Country>();

                var existingCountry = await OscarContext.Countries.FirstOrDefaultAsync(c => c.Code == country || c.Code3A == country);
                if (existingCountry != null)
                {
                    works.Countries.Add(existingCountry);
                }
            }
        }
    }
}
