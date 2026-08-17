using System;
using System.Net.Sockets;
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

namespace Oscar.Infrastructure.Features.Clients.Commands
{
    public class UpdateClientCommand : IRequest<Result>
    {
        public int Id { get; set; }
        public ClientUpdateDto ClientUpdateDto { get; set; }
    }

    public class UpdateClientCommandHandler : SimpleAbstractBaseHandler<UpdateClientCommand>
    {
        private readonly IConfiguration _config;
        private readonly ICacheService _cache;

        public UpdateClientCommandHandler(OscarContext oscarContext, IMapper mapper, 
            IValidator<UpdateClientCommand> validator, ILogger<UpdateClientCommand> logger, IConfiguration configuration, ICacheService cache) 
            : base(oscarContext, mapper, validator, logger)
        {
            _config = configuration;
            _cache = cache;
        }

        protected override async Task<Result> HandleRequest(UpdateClientCommand request, CancellationToken cancellationToken)
        {
            var client = await OscarContext.Clients
                .Include(c => c.Addresses)
                .Include(c => c.Societies)
                .Include(c => c.Contract)
                .Include(c => c.ClientAltNames)
                .Include(c=> c.Contacts)
                .Include(c => c.CustomerServiceManagers)
                .FirstOrDefaultAsync(c => c.Id == request.Id);

            if (client == null)
            {
                Logger.LogInformation((int)ClientFeatureEvent.UpdateNotFound, $"Not found {request.Id}");
                return Result.Fail<string>(CommandResult.NOTFOUND);
            }


            client.ClientReference = request.ClientUpdateDto.ClientReference;

            client.Id = request.Id;
            client.ClientName = request.ClientUpdateDto.ClientName;
            client.Status = request.ClientUpdateDto.Status;
            client.ClientGrade  = request.ClientUpdateDto.ClientGrade;
            client.ClientType = request.ClientUpdateDto.ClientType;
            client.IMaestroClientCode = request.ClientUpdateDto.IMaestroClientCode;
            client.IMaestroGroupPayeeCode = request.ClientUpdateDto.IMaestroGroupPayeeCode;
            client.IMaestroGroupPayeeName = request.ClientUpdateDto.IMaestroGroupPayeeName;
            client.Email = request.ClientUpdateDto.Email;
            client.GeneralNotes = request.ClientUpdateDto.GeneralNotes;
            client.ClientAltNames = Mapper.Map<ICollection<ClientAltName>>(request.ClientUpdateDto.ClientAltNames);
            client.AgicoaClientRef = request.ClientUpdateDto.AgicoaClientRef;
            client.CCCClientsId = request.ClientUpdateDto.CCCClientsId;
            client.CRCClientsId = request.ClientUpdateDto.CRCClientsId;
            client.MPAAClaimantsId = request.ClientUpdateDto.MPAAClaimantsId;
            client.ScreenRightsPortfolioId = request.ClientUpdateDto.ScreenRightsPortfolioId;

            if (request.ClientUpdateDto.CustomServiceManagers != null)
            {
               MapCollection(request.ClientUpdateDto!.CustomServiceManagers, client.CustomerServiceManagers);
            }

            Mapper.Map(request.ClientUpdateDto.Contract, client.Contract);

            if (request.ClientUpdateDto.Address != null && AddressHasChanged(request.ClientUpdateDto.Address, client))
            {
                client.Addresses ??= new List<Address>();
                var address = Mapper.Map<Address>(request.ClientUpdateDto.Address);
                client.Addresses.All(c => { c.IsCurrent = false; return true; });
                address.IsCurrent = true;
                client.Addresses.Add(address);
            }

            if (client.Societies == null) client.Societies = new HashSet<Core.Entities.Society>();

            foreach (var record in client.Societies)
            {
                if (!request.ClientUpdateDto.Societies.Any(a => a.Id == record.Id))
                {
                    client.Societies.Remove(record);
                }
            }

            if (request.ClientUpdateDto.Societies != null)
                foreach (SocietyDto updateSoc in request.ClientUpdateDto.Societies.Where(a =>
                             !client.Societies.Any(e => e.Id == a.Id)))
                {
                    Core.Entities.Society newSoc =
                        OscarContext.Set<Core.Entities.Society>().First(a => a.Id == updateSoc.Id);

                    if (newSoc != null)
                    {
                        client.Societies.Add(newSoc);
                    }
                }

            if (request.ClientUpdateDto!.Contacts != null)
            {
                foreach (var record in client.Contacts)
                {
                    if (!request.ClientUpdateDto!.Contacts.Any(a => a.Id == record.Id))
                    {
                        OscarContext.Contacts.Remove(record);
                    }
                }

                MapCollection(request.ClientUpdateDto!.Contacts, client.Contacts);
            }


            await OscarContext.SaveChangesAsync(cancellationToken);

            if (bool.Parse(_config["UseCache"]) == true)
            { _cache.InvalidateCacheForEntity(client); }

            Logger.LogInformation((int)ClientFeatureEvent.Update, CommandResult.SUCCESS);
            return Result.Ok(CommandResult.SUCCESS);
        }

        private bool AddressHasChanged(AddressAddDto address, Client client)
        {
            var currentAddress = client.Addresses?.FirstOrDefault(a => a.IsCurrent.GetValueOrDefault());

            return currentAddress == null ||
               currentAddress.AddressLine1 != address.AddressLine1 ||
               currentAddress.AddressLine2 != address.AddressLine2 ||
               currentAddress.AddressLine3 != address.AddressLine3 ||
               currentAddress.AddressLine4 != address.AddressLine4 ||
               currentAddress.PostZipCode != address.PostZipCode ||
               currentAddress.Country != address.Country;
        }
    }
}

