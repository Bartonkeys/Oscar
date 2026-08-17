using System;
using System.Collections;
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
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.CustomServiceManager.Queries
{
    public class GetAllOperatorsQuery: IRequest<Result<IEnumerable<OperatorDto>>>
    {
    }


    public class GetAllOperatorsQueryHandler : AbstractBaseHandler<GetAllOperatorsQuery, IEnumerable<OperatorDto>>
    {
        public GetAllOperatorsQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetAllOperatorsQuery> validator,
            ILogger<GetAllOperatorsQuery> logger)
            : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEnumerable<OperatorDto>>> HandleRequest(GetAllOperatorsQuery request, CancellationToken cancellationToken)
        {
            var operators = await OscarContext.Operators.ToListAsync(cancellationToken);

            Logger.LogInformation((int)OperatorFeatureEvent.Get, CommandResult.SUCCESS);

            return Result.Ok(operators.Select(a => Mapper.Map<OperatorDto>(a)));
        }

    }
}
