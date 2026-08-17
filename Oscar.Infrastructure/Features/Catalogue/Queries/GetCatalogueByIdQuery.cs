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

namespace Oscar.Infrastructure.Features.Catalogue.Queries
{
    public class GetCatalogueByIdQuery: BaseTableQuery, IRequest<Result<CatalogueDto>>
    {
        public int Id { get; set; }
    }

    public class CatalogueByIdHandler : AbstractBaseHandler<GetCatalogueByIdQuery, CatalogueDto>
    {
        public CatalogueByIdHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetCatalogueByIdQuery> validator, ILogger<GetCatalogueByIdQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<CatalogueDto>> HandleRequest(GetCatalogueByIdQuery request, CancellationToken cancellationToken)
        {
            var catalogue = await OscarContext.Catalogues
                .Include(i => i.Client)
                .ThenInclude(i => i.Societies)
                .Include(i => i.OtherNames)
                .Include(i => i.Mandates).ThenInclude(i => i.MandateType)
                .SingleOrDefaultAsync(w => w.Id == request.Id);
            var catalogueDto = Mapper.Map<CatalogueDto>(catalogue);
            Logger.LogInformation((int)CatalogueFeatureEvent.Get, CommandResult.SUCCESS);

            return Result.Ok(catalogueDto);
        }

    }
}
