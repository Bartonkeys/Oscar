using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using System.Linq.Expressions;

namespace Oscar.Infrastructure.Features.WorksImport.Queries
{
    public class GetWorksImportsByRequestIdQuery : BasePagingQuery,  IRequest<Result<IEntityTable<WorksImportDto>>>
    {
    }

    public class WorksImportsByRequestIdQueryHandler : AbstractBaseHandler<GetWorksImportsByRequestIdQuery, IEntityTable<WorksImportDto>>
    {
        public WorksImportsByRequestIdQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetWorksImportsByRequestIdQuery> validator, ILogger<GetWorksImportsByRequestIdQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEntityTable<WorksImportDto>>> HandleRequest(GetWorksImportsByRequestIdQuery request, CancellationToken cancellationToken)
        {
            var worksImports = OscarContext.WorksImports.Where(w => w.WorksImportRequest.Id == request.Id);
            var total = worksImports.Count();
            var pagedWorksImportRequests = worksImports
                .Skip(request.Start)
                .Take(request.Take);
            Logger.LogInformation((int)WorksImportRequestFeatureEvent.Get, CommandResult.SUCCESS);
            return Result.Ok(EntityTable<WorksImportDto>.Create(pagedWorksImportRequests.Select(c => Mapper.Map<WorksImportDto>(c))).WithTotal(total));
        }

    }
}
