using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Matching.Commands;
using System;
using System.Threading.Tasks;
using BartonKeys.Functional;
using Newtonsoft.Json;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Common.Contracts;
using Oscar.Infrastructure.Features.Report.Commands;
using Oscar.Infrastructure.Features.Report.Queries;
using Oscar.Infrastructure.Features.Report.Services;
using Oscar.Infrastructure.Features.WorksImport.Commands;

namespace Oscar.Function
{
    public class ReportingFunction
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ReportingFunction> _logger;
        private readonly IExporter _exporter;

        public ReportingFunction(IMediator mediator, ILogger<ReportingFunction> logger, IExporter exporter)
        {
            _mediator = mediator;
            _logger = logger;
            _exporter = exporter;
        }

        [FunctionName("ReportingFunction")]
        public async Task Run([QueueTrigger(QueueName.REPORTING, Connection = "oscarstorage")]string message)
        {
            var reportDataQuery = JsonConvert.DeserializeObject<GetReportDataByIdQuery>(message);
            var report = await _mediator.Send(new GetReportByIdQuery { Id = reportDataQuery.Id });

            if (report.Value.ReportStatus == ReportStatus.Building)
                return;

            var result = await _mediator.Send(reportDataQuery);

            if (result.IsSuccess)
            {
                var reportUrl = await _exporter.ExportReportsAsCsv(result.Value, $"Report_{reportDataQuery.Id}_{DateTime.Now.ToString("ddMMyyyy")}.csv");

                report.Value.ReportUrl = reportUrl.Value;
                report.Value.ReportStatus = ReportStatus.Ready;

                _logger.LogInformation((int)FunctionEvent.Match, $"Report success for queue item {reportDataQuery.Id}");
            }
            else
            {
                report.Value.ReportStatus = ReportStatus.Error;
                report.Value.Notes = result.Error;
                _logger.LogWarning((int)FunctionEvent.MatchError, $"Report failed for queue item {reportDataQuery.Id}");
            }

            report.Value.LastRunDate = DateTime.Now;

            await _mediator.Send(new EditReportCommand { ReportEditDto = report.Value });
        }
    }
}
