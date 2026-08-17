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

namespace Oscar.Infrastructure.Features.Rights.Commands
{
    public class EditRightCommand : IRequest<Result<RightDto>>
    {
        public RightAddDto RightAddDto { get; set; }
    }

    public class EditRightCommandHandler : AbstractBaseHandler<EditRightCommand, RightDto>
    {
        public EditRightCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<EditRightCommand> validator, ILogger<EditRightCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<RightDto>> HandleRequest(EditRightCommand request, CancellationToken cancellationToken)
        {
            var right = OscarContext.Rights
                .Include(i => i.ChannelRights)!.ThenInclude(cr => cr.Channel)
                .Include(i => i.LanguageRights)!.ThenInclude(lr => lr.Language)
                .Include(i => i.Countries)
                .FirstOrDefault(r => r.Id == request.RightAddDto.ID);

            right.Type = OscarContext.RightsTypes.FirstOrDefault(rt => rt.Id == request.RightAddDto.TypeID);
            right.StartOfRight = request.RightAddDto.Start;
            right.EndOfRight = request.RightAddDto.End;
            right.StartOfValidity = request.RightAddDto.StartValidity;
            right.EndOfValidity = request.RightAddDto.EndValidity;
            right.Notations = request.RightAddDto.Notations;
            right.Percentage = request.RightAddDto.Percentage;

            RightsHelper.SetChannelRights(right, request.RightAddDto.ChannelIds, OscarContext);
            RightsHelper.SetLanguageRights(right, request.RightAddDto.LanguageIds, OscarContext);
            RightsHelper.SetCollection<Core.Entities.Country>(right.Countries, request.RightAddDto.CountryIds, OscarContext);


            OscarContext.Update(right);

            await OscarContext.SaveChangesAsync(cancellationToken);

            Logger.LogInformation((int)RightFeatureEvent.Update, CommandResult.SUCCESS);
            return Result.Ok(Mapper.Map<RightDto>(right));
        }
    }
}
