using FluentValidation;
using Oscar.Core.DTOs;
using Oscar.Data.Context;

namespace Oscar.Infrastructure.Features.Clients.Validation
{


    public class RightDeleteDtoValidation : AbstractValidator<RightDeleteDto>
	{
        private OscarContext _context;

        public RightDeleteDtoValidation(OscarContext context)
		{
            _context = context;

            RuleFor(r => r.ID)
            .NotNull()
            .NotEmpty();

        }
    }
}