using AutoMapper;
using BartonKeys.Extensions;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Rights.Commands
{
    public class DeleteRightCommand : IRequest<Result<RightDto>>
    {
        public RightDeleteDto RightDeleteDto { get; set; }
    }

    public class DeleteRightCommandHandler : AbstractBaseHandler<DeleteRightCommand, RightDto>
    {
        public DeleteRightCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<DeleteRightCommand> validator, ILogger<DeleteRightCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<RightDto>> HandleRequest(DeleteRightCommand request, CancellationToken cancellationToken)
        {
            var maybeRight = OscarContext.Rights
                .Include(r => r.ChannelRights)
                .Include(r => r.LanguageRights)
                .Include(r => r.Countries)
                .FirstOrDefault(r => r.Id == request.RightDeleteDto.ID)
                .ToMaybe();

            if (!maybeRight.HasValue) return Result.Fail<RightDto>($"Right ID {request.RightDeleteDto.ID} not found");

            foreach(var cr in maybeRight.Value!.ChannelRights)
                maybeRight.Value.ChannelRights.Remove(cr);

            foreach (var l in maybeRight.Value.LanguageRights)
                maybeRight.Value.LanguageRights.Remove(l);

            foreach (var c in maybeRight.Value.Countries)
                maybeRight.Value.Countries.Remove(c);

            OscarContext.Remove(maybeRight.Value);

            await OscarContext.SaveChangesAsync(cancellationToken);

            Logger.LogInformation((int)RightFeatureEvent.Delete, CommandResult.SUCCESS);
            return Result.Ok(Mapper.Map<RightDto>(maybeRight.Value));
        }
    }
}
