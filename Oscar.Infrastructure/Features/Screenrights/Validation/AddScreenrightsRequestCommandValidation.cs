using FluentValidation;
using Microsoft.AspNetCore.Http;
using Oscar.Infrastructure.Features.Screenrights.Commands;

namespace Oscar.Infrastructure.Features.Screenrights.Validation
{
    public class AddScreenrightsRequestCommandValidation : AbstractValidator<AddScreenrightsRequestCommand>
    {
        public AddScreenrightsRequestCommandValidation()
        {
            RuleFor(r => r.ScreenrightsRequestDto).NotNull().WithMessage("Screenrights request required");
            RuleFor(r => r.ScreenrightsRequestDto.RequestedBy).NotEmpty().WithMessage("Requested by is required");
            RuleFor(r => r.ScreenrightsRequestDto.FormFile).NotNull().WithMessage("File is required");
        }
    }
}
