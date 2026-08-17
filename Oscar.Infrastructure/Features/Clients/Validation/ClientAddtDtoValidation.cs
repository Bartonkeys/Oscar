using System;
using FluentValidation;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Clients;

namespace Oscar.Infrastructure.Features.Clients.Validation
{
    

	public class CompanyAddDtoValidation : AbstractValidator<CompanyAddDto>
	{
        private OscarContext _context;

        public CompanyAddDtoValidation(OscarContext context)
		{
            _context = context;

            RuleFor(r => r.Name)
            .NotNull()
            .NotEmpty();

            RuleFor(r => r.Email)
                  .EmailAddress()
                  .WithMessage("Email must be a valid email address");
        }
    }
}