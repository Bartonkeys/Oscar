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
    public class GetReportBaseEntities: BaseTableQuery, IRequest<Result<List<string>>>
    {
    }

    public class GetReportBaseEntitiesHandler : AbstractBaseHandler<GetReportBaseEntities, List<string>>
    {
        public GetReportBaseEntitiesHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetReportBaseEntities> validator, ILogger<GetReportBaseEntities> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<List<string>>> HandleRequest(GetReportBaseEntities request, CancellationToken cancellationToken)
        {
            var baseEntityStringList = OscarContext.ReportentityJoins
                .Select(x => x.BaseEntityName).Distinct().ToList();

            Logger.LogInformation((int)ReportFeatureEvent.GetBaseEntities, CommandResult.SUCCESS);

            return Result.Ok(baseEntityStringList);
        }

    }
}
