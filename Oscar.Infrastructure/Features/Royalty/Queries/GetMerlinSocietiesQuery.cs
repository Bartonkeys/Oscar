using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Rights.Queries
{
    public class GetMerlinSocietiesQuery : IRequest<Result<List<MerlinSocietyDto>>>
    {
    }

    public class GetMerlinSocietiesQueryHandler : AbstractBaseHandler<GetMerlinSocietiesQuery, List<MerlinSocietyDto>>
    {
        public GetMerlinSocietiesQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetMerlinSocietiesQuery> validator, ILogger<GetMerlinSocietiesQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<List<MerlinSocietyDto>>> HandleRequest(GetMerlinSocietiesQuery request, CancellationToken cancellationToken)
        {
            return Result.Ok(Mapper.Map<List<MerlinSocietyDto>>(OscarContext.MerlinSocieties.ToList()));
        }
    }

}
