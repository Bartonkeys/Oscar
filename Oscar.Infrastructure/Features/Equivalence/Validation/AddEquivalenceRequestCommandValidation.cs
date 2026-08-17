using FluentValidation;
using Microsoft.AspNetCore.Http;
using Oscar.Infrastructure.Features.Equivalence.Commands;

namespace Oscar.Infrastructure.Features.Equivalence.Validation
{
    public class AddEquivalenceRequestCommandValidation : AbstractValidator<AddEquivalenceRequestCommand>
    {
        public AddEquivalenceRequestCommandValidation()
        {
            RuleFor(r => r.EquivalenceRequestDto).NotNull().WithMessage("Equivalence request required");
            RuleFor(r => r.EquivalenceRequestDto.RequestedBy).NotEmpty().WithMessage("Requested by is required");
            RuleFor(r => r.EquivalenceRequestDto.FormFile).NotNull().WithMessage("File is required");
        }
    }
}
