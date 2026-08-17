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
    public class GetAllCustomServiceManagersQuery: IRequest<Result<IEnumerable<CustomerServiceManagerDto>>>
    {
    }
    
    public class GetAllCustomServiceManagersHandler : AbstractBaseHandler<GetAllCustomServiceManagersQuery, IEnumerable<CustomerServiceManagerDto>>
    {
        public GetAllCustomServiceManagersHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetAllCustomServiceManagersQuery> validator, 
            ILogger<GetAllCustomServiceManagersQuery> logger) 
            : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEnumerable<CustomerServiceManagerDto>>> HandleRequest(GetAllCustomServiceManagersQuery request, CancellationToken cancellationToken)
        {
            var customServiceManagers = await OscarContext.CustomServiceManagers.ToListAsync(cancellationToken);

            Logger.LogInformation((int)CustomServiceManagerFeatureEvent.Get, CommandResult.SUCCESS);

            return Result.Ok(customServiceManagers.Select(a => Mapper.Map<CustomerServiceManagerDto>(a)));
        }

    }
}
