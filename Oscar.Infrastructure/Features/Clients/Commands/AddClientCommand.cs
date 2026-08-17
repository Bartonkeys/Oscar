using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Common.Contracts;
using System.Configuration;

namespace Oscar.Infrastructure.Features.Clients.Commands
{
    public class AddClientCommand: IRequest<Result<ClientDto>>
    {
        public ClientAddDto ClientAddDto { get; set; }
    }

    public class AddClientCommandHandler : AbstractBaseHandler<AddClientCommand, ClientDto>
    {
        private readonly IConfiguration _config;
        private readonly ICacheService _cache;

        public AddClientCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<AddClientCommand> validator, ILogger<AddClientCommand> logger, IConfiguration configuration, ICacheService cache) : base(oscarContext, mapper, validator, logger)
        {
            _config = configuration;
            _cache = cache;
        }

        protected override async Task<Result<ClientDto>> HandleRequest(AddClientCommand request, CancellationToken cancellationToken)
        {
            var client = Mapper.Map<Client>(request.ClientAddDto);

            var lastClientReference = OscarContext.Clients.OrderByDescending(u => u.ClientReference).FirstOrDefault();
            client.ClientReference = lastClientReference != null ? lastClientReference.ClientReference + 1 : 1;

            if (request.ClientAddDto.Address != null)
            {
                client.Addresses = new List<Address>();
                var address = Mapper.Map<Address>(request.ClientAddDto.Address);
                address.IsCurrent = true;
                client.Addresses.Add(address);  
            }

            OscarContext.Add(client);
            await OscarContext.SaveChangesAsync(cancellationToken);

            if (bool.Parse(_config["UseCache"]) == true)
            { 
                _cache.InvalidateCacheForEntity(client); 
                _cache.InvalidateCacheByKey(CacheKey.CLIENTS);
            }


            Logger.LogInformation((int)ClientFeatureEvent.Add, CommandResult.SUCCESS);
            client = await OscarContext.Clients.Include(c => c.Addresses).FirstOrDefaultAsync(c => c.Id == client.Id);
            return Result.Ok(Mapper.Map<ClientDto>(client));
        }

    }
}
