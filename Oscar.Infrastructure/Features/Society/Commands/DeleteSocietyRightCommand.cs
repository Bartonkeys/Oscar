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
    public class DeleteSocietyRightCommand: IRequest<Result>
    {
        public int SocietyId { get; set; }
        public int SocietyRightsId { get; set; }
    }

    public class DeleteSocietyRightCommandHandler : SimpleAbstractBaseHandler<DeleteSocietyRightCommand>
    {
        public DeleteSocietyRightCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<DeleteSocietyRightCommand> validator, ILogger<DeleteSocietyRightCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result> HandleRequest(DeleteSocietyRightCommand request, CancellationToken cancellationToken)
        {
            var maybeSociety = (await OscarContext
                .Societies
                .Include(s => s.SocietyRights)
                .SingleOrDefaultAsync(s => s.Id == request.SocietyId, cancellationToken: cancellationToken)).ToMaybe();

            if (!maybeSociety.HasValue)
                return Result.Fail("Society not found");

            var maybeSocietyRight = maybeSociety.Value.SocietyRights.SingleOrDefault(s => s.Id == request.SocietyRightsId)
                .ToMaybe();

            if (!maybeSocietyRight.HasValue)
                return Result.Fail("Society right not found");

            maybeSociety.Value.SocietyRights.Remove(maybeSocietyRight.Value);

            await OscarContext.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}
