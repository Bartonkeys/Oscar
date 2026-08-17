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
    public class AddReportCommand : IRequest<Result<ReportDto>>
    { 
        public ReportDto ReportAddDto { get; set; }
    }

    public class AddReportCommandHandler : AbstractBaseHandler<AddReportCommand, ReportDto>
    {
        public AddReportCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<AddReportCommand> validator, ILogger<AddReportCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<ReportDto>> HandleRequest(AddReportCommand request, CancellationToken cancellationToken)
        {
            var report = Mapper.Map<Core.Entities.Report>(request.ReportAddDto);
            if (request.ReportAddDto.ReportFields != null)
            {
                var reportFields = Mapper.Map<List<ReportField>>(request.ReportAddDto.ReportFields);
                report.ReportFields = reportFields;
            }

            report.ReportStatus = ReportStatus.NotRun;

            OscarContext.Add(report);
            await OscarContext.SaveChangesAsync(cancellationToken);

            Logger.LogInformation((int)ReportFeatureEvent.Add, CommandResult.SUCCESS);
            report = await OscarContext.Reports
                        .Include(c => c.ReportFields)
                        .FirstOrDefaultAsync(c => c.Id == report.Id);
            return Result.Ok(Mapper.Map<ReportDto>(report));
        }

    }
}
