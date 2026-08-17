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

namespace Oscar.Infrastructure.Features.Society.Commands
{
    public class AddSocietyCommand : IRequest<Result>
    {
        public SocietyDto SocietyDto { get; set; }
    }

    public class AddSocietyCommandCommandHandler : SimpleAbstractBaseHandler<AddSocietyCommand>
    {
        public AddSocietyCommandCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<AddSocietyCommand> validator, ILogger<AddSocietyCommand> logger)
            : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result> HandleRequest(AddSocietyCommand request, CancellationToken cancellationToken)
        {
            var society = Mapper.Map<Core.Entities.Society>(request.SocietyDto);
            society.Addresses = new List<Address>();
            society.Contacts = new List<Contact>();

            MapCollection(request.SocietyDto.Addresses, society.Addresses);
            MapCollection(request.SocietyDto.Contacts, society.Contacts);

            await OscarContext.Societies.AddAsync(society, cancellationToken);
            await OscarContext.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}
