using AutoMapper;
using BartonKeys.Functional;
using EntityFramework.Exceptions.Common;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Core.Providers;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Episode.Commands
{
    public class UpdateEpisodeCommand: IRequest<Result<string>>
    {
        public int Id { get; set; }
        public EpisodeUpdateDto EpisodeUpdateDto { get; set; }
    }

    public class UpdateEpisodeCommandHandler : AbstractWithMediatorHandler<UpdateEpisodeCommand, string, EpisodeUpdateDto>
    {
        private readonly IUserProvider _userProvider;

        public UpdateEpisodeCommandHandler(OscarContext oscarContext, IMapper mapper, 
            IValidator<UpdateEpisodeCommand> validator, ILogger<UpdateEpisodeCommand> logger,
            IMediator mediator, IUserProvider userProvider) : base(oscarContext, mapper, validator, logger, mediator)
        {
            _userProvider = userProvider;
        }

        protected override async Task<Result<string>> HandleRequest(UpdateEpisodeCommand request, CancellationToken cancellationToken)
        {
            var episode = OscarContext.Episodes
                .Include(i => i.Titles)
                .Include(i => i.Actors)
                .Include(i => i.Directors)
                .Include(i => i.Producers)
                .Include(i => i.ScreenWriters)
                .Include(i => i.ScriptWriters)
                .Include(i => i.Distributors)
                .Include(i => i.Clients)
                .Include(i => i.Catalogues)
                //.Include(i => i.Rights)
                .Include(i => i.Conflicts)
                .Include(i => i.WorksType)
                .Include(i => i.Countries)
                .Include(i => i.Companies)
                .Include(i => i.AlternativeTitles)
                .Include(i => i.Languages)
                .Include(i => i.SocietyReferences)!.ThenInclude(s => s.Society)
                .Include(i => i.ClientReferences)!.ThenInclude(c => c.Client)
                .Include(i => i.ReRegistrations)!.ThenInclude(s => s.Society)
                .Include(i => i.WorksStatusHistory)
                .Include(i => i.Mandates)!.ThenInclude(s => s.MandateType)
                .AsSplitQuery()
                .FirstOrDefault(s => s.Id == request.Id);

            if (episode == null)
            {
                Logger.LogInformation((int)EpisodeFeatureEvent.UpdateNotFound, $"Not found {request.Id}");
                return Result.Fail<string>(CommandResult.NOTFOUND);
            }

            episode.Id = request.Id;
            
            if (episode.WorksStatus != request.EpisodeUpdateDto.WorksStatus)
            {
                var workStatusHIstory = new WorksStatusHistory
                {
                    OldStatus = OscarContext.WorksStatuses.Single(x => x.Id == (int)episode.WorksStatus),
                    NewStatus = OscarContext.WorksStatuses.Single(x => x.Id == (int)request.EpisodeUpdateDto.WorksStatus), 
                    DateChanged = DateTime.UtcNow,
                    Username = _userProvider.GetUserName()
                };
                episode.WorksStatusHistory = new List<WorksStatusHistory>
                {
                    workStatusHIstory
                };
            }
            episode.WorksStatus = request.EpisodeUpdateDto.WorksStatus;
            
            episode.UncontrolledReason = request.EpisodeUpdateDto.UncontrolledReason;
            episode.CommissionedWorkStatus = request.EpisodeUpdateDto.CommissionedWorkStatus;
            episode.DurationMinutes = request.EpisodeUpdateDto.DurationMinutes;
            episode.ProductionYear = request.EpisodeUpdateDto.ProductionYear;
            episode.FirstBroadcastYear = request.EpisodeUpdateDto.FirstBroadcastYear;
            episode.IMaestroWorkCode = request.EpisodeUpdateDto.IMaestroWorkCode;
            episode.Isan = request.EpisodeUpdateDto.Isan;
            episode.CavcoCode = request.EpisodeUpdateDto.CavcoCode;
            episode.CrtcCode = request.EpisodeUpdateDto.CrtcCode;
            episode.GeneralNotes = request.EpisodeUpdateDto.GeneralNotes;
            episode.Number = request.EpisodeUpdateDto.Number;
            episode.GenreId = request.EpisodeUpdateDto.GenreId;
            episode.SeasonId = request.EpisodeUpdateDto.SeasonId;
            episode.SeriesId = request.EpisodeUpdateDto.SeriesId;
            episode.WorksTypeId = request.EpisodeUpdateDto.WorksTypeId;
            episode.WorksSubTypeId = request.EpisodeUpdateDto.WorksSubTypeId;
            episode.AS400RefNo = request.EpisodeUpdateDto.AS400RefNo;
            episode.CompactRef = request.EpisodeUpdateDto.CompactRef;
            episode.AgicoaWorksReference = request.EpisodeUpdateDto.AgicoaWorksReference;

            //await InheritRights(request.EpisodeUpdateDto, request.Id, episode.ProductionYear);

            WorksHelper.SetTitles(episode.Titles, request.EpisodeUpdateDto.Titles, OscarContext);
            WorksHelper.SetSocieties(episode.SocietyReferences, request.EpisodeUpdateDto.SocietyReferences, OscarContext);
            WorksHelper.SetReRegistrations(episode.ReRegistrations, request.EpisodeUpdateDto.ReRegistrations, OscarContext);
            WorksHelper.SetClients(episode.ClientReferences, episode.Clients, request.EpisodeUpdateDto.ClientReferences, OscarContext);
            WorksHelper.SetMandates(episode.Mandates, request.EpisodeUpdateDto.MandateTypes, OscarContext);

            WorksHelper.SetCollection<Core.Entities.Actor>(episode.Actors, request.EpisodeUpdateDto.ActorIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Director>(episode.Directors, request.EpisodeUpdateDto.DirectorIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Producer>(episode.Producers, request.EpisodeUpdateDto.ProducerIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.ScreenWriter>(episode.ScreenWriters, request.EpisodeUpdateDto.ScreenWriterIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.ScriptWriter>(episode.ScriptWriters, request.EpisodeUpdateDto.ScriptWriterIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Distributor>(episode.Distributors, request.EpisodeUpdateDto.DistributorIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Client>(episode.Clients, request.EpisodeUpdateDto.ClientIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Catalogue>(episode.Catalogues, request.EpisodeUpdateDto.CatalogueIds, OscarContext);
            //WorksHelper.SetCollection<Core.Entities.Right>(episode.Rights, request.EpisodeUpdateDto.RightIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Conflict>(episode.Conflicts, request.EpisodeUpdateDto.ConflictIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Country>(episode.Countries, request.EpisodeUpdateDto.CountryIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Company>(episode.Companies, request.EpisodeUpdateDto.CompanyIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.AlternativeTitle>(episode.AlternativeTitles, request.EpisodeUpdateDto.AlternativeTitleIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Language>(episode.Languages, request.EpisodeUpdateDto.LanguageIds, OscarContext);
            
            try
            {
                await OscarContext.SaveChangesAsync(cancellationToken);
            }
            catch (UniqueConstraintException e)
            {
                Logger.LogInformation((int)EpisodeFeatureEvent.Update, CommandResult.ERROR);
                var errorString = e.Message;
                if (e.InnerException != null)
                {
                    errorString += " : " + e.InnerException.Message;
                }
                return Result.Fail<string>(errorString);
            }

            Logger.LogInformation((int)EpisodeFeatureEvent.Update, CommandResult.SUCCESS);
            return Result.Ok(CommandResult.SUCCESS);
        }

    }
}
