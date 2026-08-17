using System;
using FluentValidation;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Clients;

namespace Oscar.Infrastructure.Features.Clients.Validation
{
    

	public class ClientAddtDtoValidation : AbstractValidator<ClientAddDto>
	{
        private OscarContext _context;

        public ClientAddtDtoValidation(OscarContext context)
		{
            _context = context;

            RuleFor(r => r.ClientName)
            .NotNull()
            .NotEmpty();

            RuleFor(r => r.ClientGrade)
                .NotNull()
                .WithMessage("Client grade is required")
                .IsInEnum()
                .WithMessage("Client grade must be valid value");

            RuleFor(r => r.Status)
                   .NotNull()
                   .WithMessage("Client status is required")
                   .IsInEnum()
                   .WithMessage("Client status is required");

            RuleFor(r => r.Email)
                  .EmailAddress()
                  .WithMessage("Email must be a valid email address");

            When(r => r.Address != null, () =>
            {
                #pragma warning disable CS8602 
                RuleFor(r => r.Address.AddressLine1).NotEmpty().WithMessage("Address line 1 is required when including address");
                RuleFor(r => r.Address.AddressLine2).NotEmpty().WithMessage("Address line 2 is required when including address");
                RuleFor(r => r.Address.PostZipCode).NotEmpty().WithMessage("Post/Zip Code is required when including address");
                RuleFor(r => r.Address.Country).NotEmpty().WithMessage("Country is required when including address");
            });

            When(r => r.Contract != null, () =>
            {
                RuleFor(r => r.Contract.FirstStartDate).NotNull();
                RuleFor(r => r.Contract.CurrentStartDate).NotNull();
                RuleFor(r => r.Contract.EndDate).NotNull();
            });
        }
    }
}