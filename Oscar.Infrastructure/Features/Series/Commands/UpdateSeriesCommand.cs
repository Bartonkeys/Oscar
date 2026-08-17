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

namespace Oscar.Infrastructure.Features.Series.Commands
{
    public class UpdateSeriesCommand: IRequest<Result<string>>
    {
        public int Id { get; set; }
        public SeriesUpdateDto SeriesUpdateDto { get; set; }
    }

    public class UpdateSeriesCommandHandler : AbstractWithMediatorHandler<UpdateSeriesCommand, string, SeriesUpdateDto>
    {
        private readonly IUserProvider _userProvider;

        public UpdateSeriesCommandHandler(OscarContext oscarContext, IMapper mapper, 
            IValidator<UpdateSeriesCommand> validator, ILogger<UpdateSeriesCommand> logger, IMediator mediator, IUserProvider userProvider) 
            : base(oscarContext, mapper, validator, logger, mediator)
        {
            _userProvider = userProvider;
        }

        protected override async Task<Result<string>> HandleRequest(UpdateSeriesCommand request, CancellationToken cancellationToken)
        {
            var series = OscarContext.Series
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

            if (series == null)
            {
                Logger.LogInformation((int)SeriesFeatureEvent.UpdateNotFound, $"Not found {request.Id}");
                return Result.Fail<string>(CommandResult.NOTFOUND);
            }

            series.Id = request.Id;
            // series.Titles = Mapper.Map<List<WorksTitle>>(request.SeriesUpdateDto.Titles);
            if (series.WorksStatus != request.SeriesUpdateDto.WorksStatus)
            {
                var workStatusHIstory = new WorksStatusHistory
                {
                    OldStatus = OscarContext.WorksStatuses.Single(x => x.Id == (int)series.WorksStatus),
                    NewStatus = OscarContext.WorksStatuses.Single(x => x.Id == (int)request.SeriesUpdateDto.WorksStatus),
                    DateChanged = DateTime.UtcNow,
                    Username = _userProvider.GetUserName()
                };
                series.WorksStatusHistory = new List<WorksStatusHistory>
                {
                    workStatusHIstory
                };
            }
            series.WorksStatus = request.SeriesUpdateDto.WorksStatus;
            series.UncontrolledReason = request.SeriesUpdateDto.UncontrolledReason;
            series.CommissionedWorkStatus = request.SeriesUpdateDto.CommissionedWorkStatus;
            series.DurationMinutes = request.SeriesUpdateDto.DurationMinutes;
            series.ProductionYear = request.SeriesUpdateDto.ProductionYear;
            series.FirstBroadcastYear = request.SeriesUpdateDto.FirstBroadcastYear;
            series.IMaestroWorkCode = request.SeriesUpdateDto.IMaestroWorkCode;
            series.AgicoaWorksReference = request.SeriesUpdateDto.AgicoaWorksReference;
            series.Isan = request.SeriesUpdateDto.Isan;
            series.CavcoCode = request.SeriesUpdateDto.CavcoCode;
            series.CrtcCode = request.SeriesUpdateDto.CrtcCode;
            series.GeneralNotes = request.SeriesUpdateDto.GeneralNotes;
            series.Number = request.SeriesUpdateDto.Number;
            series.GenreId = request.SeriesUpdateDto.GenreId;
            series.WorksTypeId = request.SeriesUpdateDto.WorksTypeId;
            series.WorksSubTypeId = request.SeriesUpdateDto.WorksSubTypeId;
            series.AS400RefNo = request.SeriesUpdateDto.AS400RefNo;
            series.CompactRef = request.SeriesUpdateDto.CompactRef;
            series.AgicoaWorksReference = request.SeriesUpdateDto.AgicoaWorksReference;

            //await InheritRights(request.SeriesUpdateDto, request.Id, series.ProductionYear);

            WorksHelper.SetTitles(series.Titles, request.SeriesUpdateDto.Titles, OscarContext);
            WorksHelper.SetSocieties(series.SocietyReferences, request.SeriesUpdateDto.SocietyReferences, OscarContext);
            WorksHelper.SetReRegistrations(series.ReRegistrations, request.SeriesUpdateDto.ReRegistrations, OscarContext);
            WorksHelper.SetClients(series.ClientReferences, series.Clients, request.SeriesUpdateDto.ClientReferences, OscarContext);
            WorksHelper.SetMandates(series.Mandates, request.SeriesUpdateDto.MandateTypes, OscarContext);

            WorksHelper.SetCollection<Core.Entities.Actor>(series.Actors, request.SeriesUpdateDto.ActorIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Director>(series.Directors, request.SeriesUpdateDto.DirectorIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Producer>(series.Producers, request.SeriesUpdateDto.ProducerIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.ScreenWriter>(series.ScreenWriters, request.SeriesUpdateDto.ScreenWriterIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.ScriptWriter>(series.ScriptWriters, request.SeriesUpdateDto.ScriptWriterIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Distributor>(series.Distributors, request.SeriesUpdateDto.DistributorIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Catalogue>(series.Catalogues, request.SeriesUpdateDto.CatalogueIds, OscarContext);
            //WorksHelper.SetCollection<Core.Entities.Right>(series.Rights, request.SeriesUpdateDto.RightIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Conflict>(series.Conflicts, request.SeriesUpdateDto.ConflictIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Country>(series.Countries, request.SeriesUpdateDto.CountryIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Company>(series.Companies, request.SeriesUpdateDto.CompanyIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.AlternativeTitle>(series.AlternativeTitles, request.SeriesUpdateDto.AlternativeTitleIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Language>(series.Languages, request.SeriesUpdateDto.LanguageIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Client>(series.Clients, request.SeriesUpdateDto.ClientIds, OscarContext);

            try
            {
                await OscarContext.SaveChangesAsync(cancellationToken);
            }
            catch (UniqueConstraintException e)
            {
                Logger.LogInformation((int)SeriesFeatureEvent.Update, CommandResult.ERROR);
                var errorString = e.Message;
                if (e.InnerException != null)
                {
                    errorString += " : " + e.InnerException.Message;
                }
                return Result.Fail<string>(errorString);
            }

            Logger.LogInformation((int)SeriesFeatureEvent.Update, CommandResult.SUCCESS);

            if (request.SeriesUpdateDto.UpdateAllStatus)
                await Mediator.Send(new UpdateSeriesStatusAllCommand { SeriesId = request.Id, WorksStatus = request.SeriesUpdateDto.WorksStatus}, cancellationToken);

            return Result.Ok(CommandResult.SUCCESS);
        }
    }
}
