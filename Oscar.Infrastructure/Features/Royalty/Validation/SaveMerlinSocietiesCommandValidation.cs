using FluentValidation;
using Oscar.Infrastructure.Features.Rights.Commands;

namespace Oscar.Infrastructure.Features.Rights.Validation;

public class SaveMerlinSocietiesCommandValidation : AbstractValidator<SaveMerlinSocietiesCommand>
{
    public SaveMerlinSocietiesCommandValidation()
    {
    }
}