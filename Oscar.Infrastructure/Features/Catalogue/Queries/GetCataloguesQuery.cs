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
    public class GetCataloguesQuery : IRequest<Result<IEnumerable<CatalogueDto>>>
    {
        public int ClientID { get; set; }
    }

    public class GetCataloguesHandler : AbstractBaseHandler<GetCataloguesQuery, IEnumerable<CatalogueDto>>
    {
        public GetCataloguesHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetCataloguesQuery> validator,
            ILogger<GetCataloguesQuery> logger)
            : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEnumerable<CatalogueDto>>> HandleRequest(GetCataloguesQuery request, CancellationToken cancellationToken)
        {
            var catalogues = await OscarContext.Catalogues
                .Where(c => c.Client.Id == request.ClientID)
                .ToListAsync(cancellationToken);

            Logger.LogInformation((int)StandAloneFeatureEvent.Get, CommandResult.SUCCESS);
            var mappedCat = catalogues.Select(c => Mapper.Map<CatalogueDto>(c));
            return Result.Ok(mappedCat);
        }
    }
}
