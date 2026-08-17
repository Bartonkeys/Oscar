using FluentValidation;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Actor.Validation
{
    public class ActorAddDtoValidation : AbstractValidator<ActorDto>
	{
        public ActorAddDtoValidation(OscarContext context)
		{
            var entityChecker = new EntityChecker(context);
            var validationHelper = new ValidationHelper();

            RuleFor(r => r.FirstName).NotEmpty().WithMessage("First Name is required");
            RuleFor(r => r.LastName).NotEmpty().WithMessage("Last Name is required");
        }
    }
}