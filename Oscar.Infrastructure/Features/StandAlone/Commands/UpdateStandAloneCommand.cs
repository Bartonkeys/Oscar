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

namespace Oscar.Infrastructure.Features.StandAlone.Commands
{
    public class UpdateStandAloneCommand : IRequest<Result<string?>>
    {
        public int Id { get; set; }
        public StandAloneUpdateDto StandAloneUpdateDto { get; set; }
    }

    public class UpdateStandAloneCommandHandler : AbstractWithMediatorHandler<UpdateStandAloneCommand, string?, StandAloneUpdateDto>
    {
        private readonly IUserProvider _userProvider;

        public UpdateStandAloneCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<UpdateStandAloneCommand> validator, 
            ILogger<UpdateStandAloneCommand> logger, IMediator mediator, IUserProvider userProvider) : base(oscarContext, mapper, validator, logger, mediator)
        {
            _userProvider = userProvider;
        }

        protected override async Task<Result<string>> HandleRequest(UpdateStandAloneCommand request, CancellationToken cancellationToken)
        {
            var standAlone = await OscarContext.StandAlones
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
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken: cancellationToken);

            if (standAlone == null)
            {
                Logger.LogInformation((int)StandAloneFeatureEvent.UpdateNotFound, $"Not found {request.Id}");
                return Result.Fail<string>(CommandResult.NOTFOUND);
            }

            standAlone.Id = request.Id;
            //standAlone.Titles = Mapper.Map<List<WorksTitle>>(request.StandAloneUpdateDto.Titles);
            if (standAlone.WorksStatus != request.StandAloneUpdateDto.WorksStatus)
            {
                var workStatusHIstory = new WorksStatusHistory
                {
                    OldStatus = OscarContext.WorksStatuses.Single(x => x.Id == (int)standAlone.WorksStatus),
                    NewStatus = OscarContext.WorksStatuses.Single(x => x.Id == (int)request.StandAloneUpdateDto.WorksStatus),
                    DateChanged = DateTime.UtcNow,
                    Username = _userProvider.GetUserName()
                };
                standAlone.WorksStatusHistory = new List<WorksStatusHistory>
                {
                    workStatusHIstory
                };
            }
            standAlone.WorksStatus = request.StandAloneUpdateDto.WorksStatus;
            standAlone.UncontrolledReason = request.StandAloneUpdateDto.UncontrolledReason;
            standAlone.CommissionedWorkStatus = request.StandAloneUpdateDto.CommissionedWorkStatus;
            standAlone.DurationMinutes = request.StandAloneUpdateDto.DurationMinutes;
            standAlone.ProductionYear = request.StandAloneUpdateDto.ProductionYear;
            standAlone.FirstBroadcastYear = request.StandAloneUpdateDto.FirstBroadcastYear;
            standAlone.IMaestroWorkCode = request.StandAloneUpdateDto.IMaestroWorkCode;
            standAlone.Isan = request.StandAloneUpdateDto.Isan;
            standAlone.CavcoCode = request.StandAloneUpdateDto.CavcoCode;
            standAlone.CrtcCode = request.StandAloneUpdateDto.CrtcCode;
            standAlone.GeneralNotes = request.StandAloneUpdateDto.GeneralNotes;
            standAlone.Number = request.StandAloneUpdateDto.Number;
            standAlone.GenreId = request.StandAloneUpdateDto.GenreId;
            standAlone.WorksTypeId = request.StandAloneUpdateDto.WorksTypeId;
            standAlone.WorksSubTypeId = request.StandAloneUpdateDto.WorksSubTypeId;

            standAlone.AS400RefNo = request.StandAloneUpdateDto.AS400RefNo;
            standAlone.CompactRef = request.StandAloneUpdateDto.CompactRef;
            standAlone.AgicoaWorksReference = request.StandAloneUpdateDto.AgicoaWorksReference;

            foreach (var item in request.StandAloneUpdateDto.Titles.Where(t => t.Id < 0))
                item.Id = 0;

            //await InheritRights(request.StandAloneUpdateDto, request.Id, standAlone.ProductionYear);

            WorksHelper.SetTitles(standAlone.Titles, request.StandAloneUpdateDto.Titles, OscarContext);
            WorksHelper.SetSocieties(standAlone.SocietyReferences, request.StandAloneUpdateDto.SocietyReferences, OscarContext);
            WorksHelper.SetReRegistrations(standAlone.ReRegistrations, request.StandAloneUpdateDto.ReRegistrations, OscarContext);
            WorksHelper.SetClients(standAlone.ClientReferences, standAlone.Clients, request.StandAloneUpdateDto.ClientReferences, OscarContext);
            WorksHelper.SetMandates(standAlone.Mandates, request.StandAloneUpdateDto.MandateTypes, OscarContext);

            WorksHelper.SetCollection<Core.Entities.Actor>(standAlone.Actors, request.StandAloneUpdateDto.ActorIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Director>(standAlone.Directors, request.StandAloneUpdateDto.DirectorIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Producer>(standAlone.Producers, request.StandAloneUpdateDto.ProducerIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.ScreenWriter>(standAlone.ScreenWriters, request.StandAloneUpdateDto.ScreenWriterIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.ScriptWriter>(standAlone.ScriptWriters, request.StandAloneUpdateDto.ScriptWriterIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Distributor>(standAlone.Distributors, request.StandAloneUpdateDto.DistributorIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Catalogue>(standAlone.Catalogues, request.StandAloneUpdateDto.CatalogueIds, OscarContext);
            //WorksHelper.SetCollection<Core.Entities.Right>(standAlone.Rights, request.StandAloneUpdateDto.RightIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Conflict>(standAlone.Conflicts, request.StandAloneUpdateDto.ConflictIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Country>(standAlone.Countries, request.StandAloneUpdateDto.CountryIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Company>(standAlone.Companies, request.StandAloneUpdateDto.CompanyIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.AlternativeTitle>(standAlone.AlternativeTitles, request.StandAloneUpdateDto.AlternativeTitleIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Language>(standAlone.Languages, request.StandAloneUpdateDto.LanguageIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Client>(standAlone.Clients, request.StandAloneUpdateDto.ClientIds, OscarContext);

            try
            {
                await OscarContext.SaveChangesAsync(cancellationToken);
            }
            catch (UniqueConstraintException e)
            {
                Logger.LogInformation((int)StandAloneFeatureEvent.Update, CommandResult.ERROR);
                var errorString = e.Message;
                if (e.InnerException != null){
                    errorString += " : " + e.InnerException.Message;
                }
                return Result.Fail<string>(errorString);
            }

            Logger.LogInformation((int)StandAloneFeatureEvent.Update, CommandResult.SUCCESS);
            return Result.Ok(CommandResult.SUCCESS);
        }
    }
}
