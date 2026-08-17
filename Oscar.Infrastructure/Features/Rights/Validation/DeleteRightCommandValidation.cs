using FluentValidation;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Rights.Commands;

namespace Oscar.Infrastructure.Features.Rights.Validation
{
    public class DeleteRightCommandValidation : AbstractValidator<DeleteRightCommand>
    {
        public DeleteRightCommandValidation(IValidator<RightDeleteDto>rightDeleteDtoValidator, OscarContext context)
        {
            var entityChecker = new EntityChecker(context);

            RuleFor(r => r.RightDeleteDto).NotNull();
            RuleFor(r => r.RightDeleteDto).SetValidator(rightDeleteDtoValidator);
          
        }
    }
}
