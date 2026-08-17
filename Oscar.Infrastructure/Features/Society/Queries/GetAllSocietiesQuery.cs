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

namespace Oscar.Infrastructure.Features.Society.Queries
{
    public class GetAllSocietiesQuery : IRequest<Result<IEnumerable<SocietyDto>>>
    {
    }
    
    public class GetAllSocietiesHandler : AbstractBaseHandler<GetAllSocietiesQuery, IEnumerable<SocietyDto>>
    {
        public GetAllSocietiesHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetAllSocietiesQuery> validator, 
            ILogger<GetAllSocietiesQuery> logger) 
            : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEnumerable<SocietyDto>>> HandleRequest(GetAllSocietiesQuery request, CancellationToken cancellationToken)
        {
            var societies = OscarContext.Societies
                .Include(a => a.Addresses)
                .AsNoTracking()
                .ToList();

            Logger.LogInformation((int)SocietyFeatureEvent.Get, CommandResult.SUCCESS);

            return Result.Ok(societies.Select(a => Mapper.Map<SocietyDto>(a)));
        }

    }
}
