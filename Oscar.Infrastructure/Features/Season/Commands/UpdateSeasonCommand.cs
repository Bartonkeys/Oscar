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
using Oscar.Infrastructure.Features.Series.Commands;

namespace Oscar.Infrastructure.Features.Season.Commands
{
    public class UpdateSeasonCommand: IRequest<Result<string>>
    {
        public int Id { get; set; }
        public SeasonUpdateDto SeasonUpdateDto { get; set; }
    }

    public class UpdateSeasonCommandHandler : AbstractWithMediatorHandler<UpdateSeasonCommand, string, SeasonUpdateDto>
    {
        private readonly IUserProvider _userProvider;

        public UpdateSeasonCommandHandler(OscarContext oscarContext, IMapper mapper, 
            IValidator<UpdateSeasonCommand> validator, ILogger<UpdateSeasonCommand> logger, IMediator mediator, IUserProvider userProvider) 
            : base(oscarContext, mapper, validator, logger, mediator)
        {
            _userProvider = userProvider;
        }

        protected override async Task<Result<string>> HandleRequest(UpdateSeasonCommand request, CancellationToken cancellationToken)
        {
            var season = OscarContext.Seasons
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
                .Include(i => i.Mandates)!.ThenInclude(s => s.MandateType)
                .AsSplitQuery()
                .FirstOrDefault(s => s.Id == request.Id);
            if (season == null)
            {
                Logger.LogInformation((int)SeasonFeatureEvent.UpdateNotFound, $"Not found {request.Id}");
                return Result.Fail<string>(CommandResult.NOTFOUND);
            }

            season.Id = request.Id;

            try
            {

                if (season.WorksStatus != request.SeasonUpdateDto.WorksStatus)
                {
                    var workStatusHIstory = new WorksStatusHistory
                    {
                        OldStatus = OscarContext.WorksStatuses.Single(x => x.Id == (int)season.WorksStatus),
                        NewStatus = OscarContext.WorksStatuses.Single(x => x.Id == (int)request.SeasonUpdateDto.WorksStatus),
                        DateChanged = DateTime.UtcNow,
                        Username = _userProvider.GetUserName()
                    };
                    season.WorksStatusHistory = new List<WorksStatusHistory>
                {
                    workStatusHIstory
                };
                }
            }
            catch (Exception ex)
            {
                var x = ex.InnerException;
            }

            season.WorksStatus = request.SeasonUpdateDto.WorksStatus;
            season.UncontrolledReason = request.SeasonUpdateDto.UncontrolledReason;
            season.CommissionedWorkStatus = request.SeasonUpdateDto.CommissionedWorkStatus;
            season.DurationMinutes = request.SeasonUpdateDto.DurationMinutes;
            season.ProductionYear = request.SeasonUpdateDto.ProductionYear;
            season.FirstBroadcastYear = request.SeasonUpdateDto.FirstBroadcastYear;
            season.IMaestroWorkCode = request.SeasonUpdateDto.IMaestroWorkCode;
            season.AgicoaWorksReference = request.SeasonUpdateDto.AgicoaWorksReference;
            season.Isan = request.SeasonUpdateDto.Isan;
            season.CavcoCode = request.SeasonUpdateDto.CavcoCode;
            season.CrtcCode = request.SeasonUpdateDto.CrtcCode;
            season.GeneralNotes = request.SeasonUpdateDto.GeneralNotes;
            season.Number = request.SeasonUpdateDto.Number;
            season.GenreId = request.SeasonUpdateDto.GenreId;
            season.SeriesId = request.SeasonUpdateDto.SeriesId;
            season.WorksTypeId = request.SeasonUpdateDto.WorksTypeId;
            season.WorksSubTypeId = request.SeasonUpdateDto.WorksSubTypeId;
            season.AS400RefNo = request.SeasonUpdateDto.AS400RefNo;
            season.CompactRef = request.SeasonUpdateDto.CompactRef;
            season.AgicoaWorksReference = request.SeasonUpdateDto.AgicoaWorksReference;

            //await InheritRights(request.SeasonUpdateDto, request.Id, season.ProductionYear);

            WorksHelper.SetTitles(season.Titles, request.SeasonUpdateDto.Titles, OscarContext);
            WorksHelper.SetSocieties(season.SocietyReferences, request.SeasonUpdateDto.SocietyReferences, OscarContext);
            WorksHelper.SetReRegistrations(season.ReRegistrations, request.SeasonUpdateDto.ReRegistrations, OscarContext);
            WorksHelper.SetClients(season.ClientReferences, season.Clients, request.SeasonUpdateDto.ClientReferences, OscarContext);
            WorksHelper.SetMandates(season.Mandates, request.SeasonUpdateDto.MandateTypes, OscarContext);

            WorksHelper.SetCollection<Core.Entities.Actor>(season.Actors, request.SeasonUpdateDto.ActorIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Director>(season.Directors, request.SeasonUpdateDto.DirectorIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Producer>(season.Producers, request.SeasonUpdateDto.ProducerIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.ScreenWriter>(season.ScreenWriters, request.SeasonUpdateDto.ScreenWriterIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.ScriptWriter>(season.ScriptWriters, request.SeasonUpdateDto.ScriptWriterIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Distributor>(season.Distributors, request.SeasonUpdateDto.DistributorIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Client>(season.Clients, request.SeasonUpdateDto.ClientIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Catalogue>(season.Catalogues, request.SeasonUpdateDto.CatalogueIds, OscarContext);
            //WorksHelper.SetCollection<Core.Entities.Right>(season.Rights, request.SeasonUpdateDto.RightIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Conflict>(season.Conflicts, request.SeasonUpdateDto.ConflictIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Country>(season.Countries, request.SeasonUpdateDto.CountryIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Company>(season.Companies, request.SeasonUpdateDto.CompanyIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.AlternativeTitle>(season.AlternativeTitles, request.SeasonUpdateDto.AlternativeTitleIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Language>(season.Languages, request.SeasonUpdateDto.LanguageIds, OscarContext);

            try
            {
                await OscarContext.SaveChangesAsync(cancellationToken);
            }
            catch (UniqueConstraintException e)
            {
                Logger.LogInformation((int)SeasonFeatureEvent.Update, CommandResult.ERROR);
                var errorString = e.Message;
                if (e.InnerException != null)
                {
                    errorString += " : " + e.InnerException.Message;
                }
                return Result.Fail<string>(errorString);
            }

            if (request.SeasonUpdateDto.UpdateAllStatus)
                await Mediator.Send(new UpdateSeasonStatusAllCommand() { SeasonId = request.Id, WorksStatus = request.SeasonUpdateDto.WorksStatus}, cancellationToken);

            Logger.LogInformation((int)SeasonFeatureEvent.Update, CommandResult.SUCCESS);
            return Result.Ok(CommandResult.SUCCESS);
        }

    }
}
