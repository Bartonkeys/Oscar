using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Rights.Queries
{
    public class GetRightsTypeQuery : IRequest<Result<IEnumerable<RightsTypeDto>>>
    {
    }

    public class GetRightsTypeHandler : AbstractBaseHandler<GetRightsTypeQuery, IEnumerable<RightsTypeDto>>
    {
        public GetRightsTypeHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetRightsTypeQuery> validator, ILogger<GetRightsTypeQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEnumerable<RightsTypeDto>>> HandleRequest(GetRightsTypeQuery request, CancellationToken cancellationToken)
        {
            var rights = await OscarContext
                .RightsTypes.AsNoTracking()
                .ToListAsync(cancellationToken);

            var results = rights.Select(r => Mapper.Map<RightsTypeDto>(r)).ToList();

            results.Add(new RightsTypeDto
            {
                Id = 99,
                Name = "All",
                Description = "All Rights Types"
            });

            return Result.Ok(results.AsEnumerable());
        }
    }

}
