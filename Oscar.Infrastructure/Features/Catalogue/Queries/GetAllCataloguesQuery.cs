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
using System.Linq.Expressions;
using Oscar.Infrastructure.Features.Common.Contracts;

namespace Oscar.Infrastructure.Features.Catalogue.Queries
{
    public class GetAllCataloguesQuery : IRequest<Result<IEnumerable<CatalogueDto>>>
    {
    }
    
    public class GetAllCataloguesHandler : AbstractBaseHandler<GetAllCataloguesQuery, IEnumerable<CatalogueDto>>
    {
        public GetAllCataloguesHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetAllCataloguesQuery> validator, 
            ILogger<GetAllCataloguesQuery> logger) 
            : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEnumerable<CatalogueDto>>> HandleRequest(GetAllCataloguesQuery request, CancellationToken cancellationToken)
        {
            var catalogues = OscarContext.Catalogues.ToList();

            Logger.LogInformation((int)CatalogueFeatureEvent.Get, CommandResult.SUCCESS);

            return Result.Ok(catalogues.Select(c => Mapper.Map<CatalogueDto>(c)));
        }

    }
}
