using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BartonKeys.Extensions;
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
    public class AddSocietyRightCommand: IRequest<Result>
    {
        public int SocietyId { get; set; }
        public SocietyRightsDto SocietyRightsDto { get; set; }
    }

    public class AddSocietyRightCommandHandler : SimpleAbstractBaseHandler<AddSocietyRightCommand>
    {
        public AddSocietyRightCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<AddSocietyRightCommand> validator, ILogger<AddSocietyRightCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result> HandleRequest(AddSocietyRightCommand request, CancellationToken cancellationToken)
        {
            var maybeSociety = (await OscarContext
                .Societies
                .Include(s => s.SocietyRights)
                .SingleOrDefaultAsync(s => s.Id == request.SocietyId, cancellationToken: cancellationToken)).ToMaybe();

            if (!maybeSociety.HasValue)
                return Result.Fail("Society not found");

            var societyRight = new SocietyRights
            {
                RightsType = await OscarContext.RightsTypes.SingleAsync(r => r.Id == request.SocietyRightsDto.RightsType.Id),
                Country = await OscarContext.Countries.SingleAsync(c => c.Id == request.SocietyRightsDto.Country.Id)
            };

            maybeSociety.Value.SocietyRights.Add(societyRight);

            await OscarContext.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}
