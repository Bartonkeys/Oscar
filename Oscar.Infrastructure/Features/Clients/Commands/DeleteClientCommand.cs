using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Common.Contracts;
using System.Configuration;

namespace Oscar.Infrastructure.Features.Clients.Commands
{
    public class DeleteClientCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }
    }

    public class DeleteClientCommandHandler : AbstractBaseHandler<DeleteClientCommand, bool>
    {
        private readonly IConfiguration _config;
        private readonly ICacheService _cache;

        public DeleteClientCommandHandler(OscarContext oscarContext, IMapper mapper, 
            IValidator<DeleteClientCommand> validator, ILogger<DeleteClientCommand> logger, IConfiguration configuration, ICacheService cache) 
            : base(oscarContext, mapper, validator, logger)
        {
            _config = configuration;
            _cache = cache;
        }

        protected override async Task<Result<bool>> HandleRequest(DeleteClientCommand request, CancellationToken cancellationToken)
        {
            var clientEntity = OscarContext.Clients.FirstOrDefault(x => x.Id == request.Id);

            if (clientEntity == null)
            {
                return Result.Fail<bool>("Client not found");
            }

            OscarContext.Clients.Remove(clientEntity);
            await OscarContext.SaveChangesAsync();

            if (bool.Parse(_config["UseCache"]) == true)
            { _cache.InvalidateCacheForEntity(clientEntity); }

            Logger.LogInformation((int)ClientFeatureEvent.Delete, CommandResult.SUCCESS);
            return Result.Ok(Mapper.Map<bool>(true));
        }
    }
}

