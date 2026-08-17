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

namespace Oscar.Infrastructure.Features.Channel.Queries
{
    public class GetAllChannelsQuery: IRequest<Result<IEnumerable<ChannelDto>>>
    {
    }
    
    public class GetAllChannelsHandler : AbstractBaseHandler<GetAllChannelsQuery, IEnumerable<ChannelDto>>
    {
        public GetAllChannelsHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetAllChannelsQuery> validator, 
            ILogger<GetAllChannelsQuery> logger) 
            : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEnumerable<ChannelDto>>> HandleRequest(GetAllChannelsQuery request, CancellationToken cancellationToken)
        {
            var channels = OscarContext.Channel.ToList();

            Logger.LogInformation((int)ChannelFeatureEvent.Get, CommandResult.SUCCESS);

            return Result.Ok(channels.Select(a => Mapper.Map<ChannelDto>(a)));
        }

    }
}
