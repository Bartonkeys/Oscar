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

namespace Oscar.Infrastructure.Features.Distributor.Queries
{
    public class GetDistributorQuery : BaseTableQuery, IRequest<Result<IEntityTable<DistributorDto>>>
    {
        public int Id { get; set; }
    }

    public class GetDistributorQueryHandler : AbstractBaseHandler<GetDistributorQuery, IEntityTable<DistributorDto>>
    {
        public GetDistributorQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetDistributorQuery> validator, ILogger<GetDistributorQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEntityTable<DistributorDto>>> HandleRequest(GetDistributorQuery request, CancellationToken cancellationToken)
        {
            Logger.LogInformation((int)DistributorFeatureEvent.Get, CommandResult.SUCCESS);

            var distributors = OscarContext.Distributors;
            var total = distributors.Count();

            return Result.Ok(EntityTable<DistributorDto>.Create(distributors.Select(c => Mapper.Map<DistributorDto>(c))).WithTotal(total));
        }
        
    }
}
