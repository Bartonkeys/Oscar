using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Common.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Oscar.Infrastructure.Features.WorksImport.Queries
{
    public class GetWorksImportRequestsQuery : BaseTableQuery, IRequest<Result<IEntityTable<WorksImportRequestDto>>>
    {
        public GetWorksImportRequestsQuery()
        {
            SearchObjects.Add(new SearchObject(
                "WorksImportRequest",
                "string",
                "reference",
                ""
            ));
        }
    }

    public class WorksImportRequestsQueryHandler : AbstractBaseHandler<GetWorksImportRequestsQuery, IEntityTable<WorksImportRequestDto>>
    {
        IDynamicExpressionBuilderService DynamicExpressionBuilderService;

        public WorksImportRequestsQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetWorksImportRequestsQuery> validator, ILogger<GetWorksImportRequestsQuery> logger, IDynamicExpressionBuilderService dynamicExpressionBuilderService) : base(oscarContext, mapper, validator, logger)
        {
            DynamicExpressionBuilderService = dynamicExpressionBuilderService;
        }

        protected override async Task<Result<IEntityTable<WorksImportRequestDto>>> HandleRequest(GetWorksImportRequestsQuery request, CancellationToken cancellationToken)
        {
            var worksImportRequests = OscarContext.WorksImportRequests
                .Include(c => c.Client)
                .Include(cat => cat.Catalogue)
                .AsNoTracking().Select(r => new WorksImportRequestDto
                {
                    ClientName = r.Client.ClientName ?? string.Empty,
                    ClientId = r.Client.Id,
                    CatalogueName = (r.Catalogue != null ? r.Catalogue.Name : null) ?? "N/A",
                    CatalogueId = r.CatalogueId,
                    Id = r.Id,
                    Reference = r.Reference,
                    RequestedBy = r.RequestedBy,
                    Status = r.Status,
                    CreationDate = r.CreationDate,
                    LastModified = r.LastModified
                })
                .OrderByDescending(r => r.Id)
                .ToList();

            var total = worksImportRequests.Count();
            var pagedWorksImportRequests = worksImportRequests
                .Skip(request.Start)
                .Take(request.Take);

            Logger.LogInformation((int)WorksImportRequestFeatureEvent.Get, CommandResult.SUCCESS);
            return Result.Ok(EntityTable<WorksImportRequestDto>.Create(pagedWorksImportRequests).WithTotal(total));
        }
    }
}
