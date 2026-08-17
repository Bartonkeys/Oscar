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

namespace Oscar.Infrastructure.Features.Catalogue.Queries
{
    public class GetCatalogueQuery : BaseTableQuery, IRequest<Result<IEntityTable<CatalogueDto>>>
    {
        public int Id { get; set; }
    }

    public class GetCatalogueQueryHandler : AbstractBaseHandler<GetCatalogueQuery, IEntityTable<CatalogueDto>>
    {
        public GetCatalogueQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetCatalogueQuery> validator, ILogger<GetCatalogueQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEntityTable<CatalogueDto>>> HandleRequest(GetCatalogueQuery request, CancellationToken cancellationToken)
        {
            Logger.LogInformation((int)CatalogueFeatureEvent.Get, CommandResult.SUCCESS);

            var catalogues = OscarContext.Catalogues;
            var total = catalogues.Count();

            return Result.Ok(EntityTable<CatalogueDto>.Create(catalogues.Select(c => Mapper.Map<CatalogueDto>(c))).WithTotal(total));
        }
        
    }
}
