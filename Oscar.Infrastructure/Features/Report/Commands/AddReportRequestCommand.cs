using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Core.Entities;
using Oscar.Data.Context;
using System.Linq.Expressions;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Report.Services;
using System.Dynamic;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;
using Oscar.Infrastructure.Features.Common.Contracts;

namespace Oscar.Infrastructure.Features.Report.Queries
{
    public class AddReportRequestCommand : BaseTableQuery, IRequest<Result>
    {
        public int Id { get; set; }
    }

    public class AddReportRequestCommandHandler : SimpleAbstractBaseHandler<AddReportRequestCommand>
    {
        private readonly IQueueService _queueService;

        public AddReportRequestCommandHandler(OscarContext oscarContext, IMapper mapper, 
            IValidator<AddReportRequestCommand> validator, ILogger<AddReportRequestCommand> logger, IQueueService queueService) : base(oscarContext, mapper, validator, logger)
        {
            _queueService = queueService;
        }

        protected override async Task<Result> HandleRequest(AddReportRequestCommand request, CancellationToken cancellationToken)
        {
            var report = await OscarContext.Reports.FindAsync(request.Id);
            report.ReportStatus = ReportStatus.Queued;
            await OscarContext.SaveChangesAsync(cancellationToken);

            await _queueService.SendAsync(QueueName.REPORTING, JsonConvert.SerializeObject(request), cancellationToken);
            return Result.Ok();
        }

    }
}
