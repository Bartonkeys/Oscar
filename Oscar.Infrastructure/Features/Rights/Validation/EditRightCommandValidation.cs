using FluentValidation;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Rights.Commands;

namespace Oscar.Infrastructure.Features.Rights.Validation
{
    public class EditRightCommandValidation : AbstractValidator<EditRightCommand>
    {
        public EditRightCommandValidation(IValidator<RightAddDto>rightEditDtoValidator, OscarContext context)
        {
            var entityChecker = new EntityChecker(context);

            RuleFor(r => r.RightAddDto).NotNull();
            RuleFor(r => r.RightAddDto).SetValidator(rightEditDtoValidator);
          
        }
    }
}
