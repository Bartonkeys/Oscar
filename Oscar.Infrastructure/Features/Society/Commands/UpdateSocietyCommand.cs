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

namespace Oscar.Infrastructure.Features.Society.Commands
{
    public class 
        UpdateSocietyCommand: IRequest<Result>
    {
        public SocietyDto SocietyDto { get; set; }
    }

    public class UpdateSocietyCommandHandler : SimpleAbstractBaseHandler<UpdateSocietyCommand>
    {
        public UpdateSocietyCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<UpdateSocietyCommand> validator, ILogger<UpdateSocietyCommand> logger) 
            : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result> HandleRequest(UpdateSocietyCommand request, CancellationToken cancellationToken)
        {
            var society = await OscarContext.Societies
                .Include(c => c.Contacts)
                .Include(c => c.Addresses)
                .SingleOrDefaultAsync(s => s.Id == request.SocietyDto.Id, cancellationToken: cancellationToken);

            Mapper.Map(request.SocietyDto, society);

            foreach (var record in society.Contacts)
            {
                if (!request.SocietyDto.Contacts.Any(a => a.Id == record.Id))
                {
                    OscarContext.Contacts.Remove(record);
                }
            }

            //MapCollection(request.SocietyDto.Clients, society.Clients);
            MapCollection(request.SocietyDto.Contacts, society.Contacts);
            MapCollection(request.SocietyDto.Addresses, society.Addresses);
            //MapCollection(request.SocietyDto.SocietyRights, society.SocietyRights);

            await OscarContext.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}
