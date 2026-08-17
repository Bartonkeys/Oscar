using FluentValidation;
using Oscar.Core.DTOs;
using Oscar.Data.Context;

namespace Oscar.Infrastructure.Features.Clients.Validation
{


    public class RightAddDtoValidation : AbstractValidator<RightAddDto>
	{
        private OscarContext _context;

        public RightAddDtoValidation(OscarContext context)
		{
            _context = context;

        }
    }
}