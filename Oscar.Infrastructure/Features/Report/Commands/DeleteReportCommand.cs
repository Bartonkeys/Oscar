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
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Report.Services;


namespace Oscar.Infrastructure.Features.Report.Commands
{
    public class DeleteReportCommand: IRequest<Result<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteReportCommandHandler : AbstractBaseHandler<DeleteReportCommand, string>
    {
        public DeleteReportCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<DeleteReportCommand> validator, ILogger<DeleteReportCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<string>> HandleRequest(DeleteReportCommand request, CancellationToken cancellationToken)
        {
            var report = OscarContext.Reports
                .Include(e => e.ReportFields)
                .FirstOrDefault(s => s.Id == request.Id);

            if (report == null)
            {
                Logger.LogInformation((int)ReportFeatureEvent.DeleteNotFound, $"Not found {request.Id}");
                return Result.Fail<string>(CommandResult.NOTFOUND);
            }

            ReportHelperService.DeleteReport(report, OscarContext);
            await OscarContext.SaveChangesAsync(cancellationToken);

            Logger.LogInformation((int)ReportFeatureEvent.Delete, CommandResult.SUCCESS);
            return Result.Ok(CommandResult.SUCCESS);
        }

        
    }
}
