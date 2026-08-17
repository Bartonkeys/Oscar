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

namespace Oscar.Infrastructure.Features.Report.Commands
{
    public class EditReportCommand : IRequest<Result<ReportDto>>
    { 
        public ReportDto ReportEditDto { get; set; }
    }

    public class EditReportCommandHandler : AbstractBaseHandler<EditReportCommand, ReportDto>
    {
        public EditReportCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<EditReportCommand> validator, ILogger<EditReportCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<ReportDto>> HandleRequest(EditReportCommand request, CancellationToken cancellationToken)
        {

            var report = OscarContext.Reports
                .Include(i => i.ReportFields)
                .FirstOrDefault(w => w.Id == request.ReportEditDto.Id);

            if (report != null)
            {
                report.ReportFields = Mapper.Map<List<ReportField>>(request.ReportEditDto.ReportFields);
                report.ReportName = request.ReportEditDto.ReportName;
                report.BaseEntityName = request.ReportEditDto.BaseEntityName;
                report.ReportUrl = request.ReportEditDto.ReportUrl;
                report.ReportStatus = request.ReportEditDto.ReportStatus;
                report.LastRunDate = request.ReportEditDto.LastRunDate;
            }


            OscarContext.Update(report);
            await OscarContext.SaveChangesAsync(cancellationToken);

            Logger.LogInformation((int)ReportFeatureEvent.Update, CommandResult.SUCCESS);
            report = await OscarContext.Reports
                        .Include(c => c.ReportFields)
                        .FirstOrDefaultAsync(c => c.Id == report.Id);
            return Result.Ok(Mapper.Map<ReportDto>(report));
        }

    }
}
