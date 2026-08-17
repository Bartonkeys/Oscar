using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Extensions;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Episode.Commands
{
    public class BulkAddEpisodeCommand: IRequest<Result<string>>
    {
        public ICollection<TitleLanguageDto>? EpisodeTitles { get; set; }
        public int? SeasonId { get; set; }
        public int? SeriesId { get; set; }
    }

    public class BulkAddEpisodeCommandHandler : AbstractBaseHandler<BulkAddEpisodeCommand, string>
    {
        private readonly IMediator _mediator;

        public BulkAddEpisodeCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<BulkAddEpisodeCommand> validator, 
            ILogger<BulkAddEpisodeCommand> logger, IMediator mediator) : base(oscarContext, mapper, validator, logger)
        {
            _mediator = mediator;
        }

        protected override async Task<Result<string>> HandleRequest(BulkAddEpisodeCommand request, CancellationToken cancellationToken)
        {
            if (request.SeasonId != null)
            {
                var parentSeason = await OscarContext.Seasons
                .AsNoTracking()
                .Include(i => i.Genre)
                .Include(i => i.WorksType)
                .Include(i => i.Countries)
                .Include(i => i.Companies)
                .Include(r => r.Rights)!.ThenInclude(c => c.LanguageRights).ThenInclude(l => l.Language)
                .Include(r => r.Rights)!.ThenInclude(c => c.ChannelRights).ThenInclude(c => c.Channel)
                .Include(w => w.Rights)!.ThenInclude(c => c.Type)
                .Include(w => w.Rights)!.ThenInclude(c => c.Countries)
                .Include(i => i.Languages)
                .Include(i => i.Producers)
                .Include(i => i.Directors)
                .Include(i => i.Actors)
                .Include(i => i.Distributors)
                .Include(i => i.ScreenWriters)
                .Include(i => i.ScriptWriters)
                .Include(i => i.Clients)
                .Include(i => i.Catalogues)
                .AsSplitQuery()
                .SingleOrDefaultAsync(w => w.Id == request.SeasonId, cancellationToken);

                if (request.EpisodeTitles != null && parentSeason != null)
                {
                    ICollection<Core.Entities.Episode> newEpisodes = new List<Core.Entities.Episode>();

                    int counter = 1;
                    foreach (var item in request.EpisodeTitles)
                    {
                        ICollection<Core.Entities.WorksTitle> titles = new List<Core.Entities.WorksTitle>();
                        titles.Add(new WorksTitle { Id = 0, LanguageCode = item.Language?.Name, Title = item.Title, TitleType = TitleType.Main });

                        var newEpisode = new Core.Entities.Episode();
                        newEpisode.SeasonId = request.SeasonId;
                        newEpisode.SeriesId = request.SeriesId;
                        newEpisode.Titles = titles;
                        newEpisode.Id = 0;
                        newEpisode.WorksStatus = parentSeason.WorksStatus;
                        newEpisode.CommissionedWorkStatus = parentSeason.CommissionedWorkStatus;
                        newEpisode.GenreId = parentSeason.GenreId;
                        newEpisode.DurationMinutes = parentSeason.DurationMinutes;
                        newEpisode.ProductionYear = parentSeason.ProductionYear;
                        newEpisode.FirstBroadcastYear = parentSeason.FirstBroadcastYear;
                        newEpisode.IMaestroWorkCode = parentSeason.IMaestroWorkCode;
                        newEpisode.Isan = parentSeason.Isan;
                        newEpisode.Number = item.EpisodeNumber;
                        newEpisode.WorksTypeId = parentSeason.WorksTypeId;
                        newEpisode.GenreSubTypeId = parentSeason.GenreSubTypeId;
                        newEpisode.WorksSubTypeId = parentSeason.WorksSubTypeId;
                        newEpisode.CompactRef = AutoGenerateCompactRef(counter);
                        counter++;

                        newEpisode.Actors = parentSeason.Actors?.Load(OscarContext);
                        newEpisode.Actors = new List<Core.Entities.Actor>();
                        WorksHelper.SetCollection<Core.Entities.Actor>(newEpisode.Actors, parentSeason.Actors?.Select(c => c.Id).ToList(), OscarContext);
                        newEpisode.Companies = new List<Core.Entities.Company>();
                        WorksHelper.SetCollection<Core.Entities.Company>(newEpisode.Companies, parentSeason.Companies?.Select(c => c.Id).ToList(), OscarContext);
                        newEpisode.Conflicts = parentSeason.Conflicts?.Clone(OscarContext);
                        newEpisode.Countries = new List<Core.Entities.Country>();
                        WorksHelper.SetCollection<Core.Entities.Country>(newEpisode.Countries, parentSeason.Countries?.Select(c => c.Id).ToList(), OscarContext);
                        newEpisode.Directors = new List<Core.Entities.Director>();
                        WorksHelper.SetCollection<Core.Entities.Director>(newEpisode.Directors, parentSeason.Directors?.Select(c => c.Id).ToList(), OscarContext);
                        newEpisode.Distributors = new List<Core.Entities.Distributor>();
                        WorksHelper.SetCollection<Core.Entities.Distributor>(newEpisode.Distributors, parentSeason.Distributors?.Select(c => c.Id).ToList(), OscarContext);
                        newEpisode.Rights = parentSeason.Rights?.CloneRights(OscarContext);
                        newEpisode.Languages = new List<Core.Entities.Language>();
                        WorksHelper.SetCollection<Core.Entities.Language>(newEpisode.Languages, parentSeason.Languages?.Select(c => c.Id).ToList(), OscarContext);
                        newEpisode.Producers = new List<Core.Entities.Producer>();
                        WorksHelper.SetCollection<Core.Entities.Producer>(newEpisode.Producers, parentSeason.Producers?.Select(c => c.Id).ToList(), OscarContext);
                        newEpisode.ScreenWriters = new List<Core.Entities.ScreenWriter>();
                        WorksHelper.SetCollection<Core.Entities.ScreenWriter>(newEpisode.ScreenWriters, parentSeason.ScreenWriters?.Select(c => c.Id).ToList(), OscarContext);
                        newEpisode.Clients = new List<Core.Entities.Client>();
                        WorksHelper.SetCollection<Core.Entities.Client>(newEpisode.Clients, parentSeason.Clients?.Select(c => c.Id).ToList(), OscarContext);
                        newEpisode.Catalogues = new List<Core.Entities.Catalogue>();
                        WorksHelper.SetCollection<Core.Entities.Catalogue>(newEpisode.Catalogues, parentSeason.Catalogues?.Select(c => c.Id).ToList(), OscarContext);
                        newEpisode.ClientReferences = new List<ClientReference> { new ClientReference { Works = newEpisode, Client = newEpisode.Clients.First(), Catalogue = newEpisode.Catalogues.First() } };

                        newEpisode.Mandates = new List<Core.Entities.Mandate>();
                        var mandateTypes = OscarContext.MandateType.AsNoTracking().ToList();
                        foreach (var mandateType in mandateTypes)
                        {
                            var newMandate = new Mandate();
                            newMandate.MandateType = OscarContext.MandateType.First(x => x.Id == mandateType.Id);
                            newMandate.Mandated = true;
                            newEpisode.Mandates.Add(newMandate);
                        }

                        newEpisodes.Add(newEpisode);
                    }

                    OscarContext.Works.AddRange(newEpisodes);
                    await OscarContext.SaveChangesAsync(cancellationToken);

                }
            }

            Logger.LogInformation((int)EpisodeFeatureEvent.BulkAdd, CommandResult.SUCCESS);
            return Result.Ok(CommandResult.SUCCESS);
        }

    }
}
