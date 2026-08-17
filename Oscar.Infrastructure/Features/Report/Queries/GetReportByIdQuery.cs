using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Report.Queries
{
    public class GetReportByIdQuery: BaseTableQuery, IRequest<Result<ReportDto>>
    {
        public int Id { get; set; }
    }

    public class ReportByIdHandler : AbstractBaseHandler<GetReportByIdQuery, ReportDto>
    {
        public ReportByIdHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetReportByIdQuery> validator, ILogger<GetReportByIdQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<ReportDto>> HandleRequest(GetReportByIdQuery request, CancellationToken cancellationToken)
        {
            var report = await OscarContext.Reports
                .Include(i => i.ReportFields)
                .SingleOrDefaultAsync(w => w.Id == request.Id);

            var reportDto = Mapper.Map<ReportDto>(report);
            Logger.LogInformation((int)ReportFeatureEvent.Get, CommandResult.SUCCESS);

            return Result.Ok(reportDto);
        }

    }
}
