using FluentValidation;
using Microsoft.Extensions.Options;
using Oscar.MRIT.Core.Configuration;
using Oscar.Mrit.Features.FelixMrit.Commands;

namespace Oscar.Mrit.Features.FelixMrit.Validation
{
    public class AddFelixMritMatchesCommandValidator : AbstractValidator<AddFelixMritMatchesCommand>
    {
        public AddFelixMritMatchesCommandValidator(IOptions<BatchSettings> batchSettings)
        {
            RuleFor(m => m.Matches).NotNull().WithMessage("Match collection is null");

            if (batchSettings.Value.Size == 0)
            {
                batchSettings.Value.Size = 20;
            }
            
            When(m => m.Matches != null, () =>
            {
                RuleFor(m => m.Matches).NotEmpty().WithMessage("Match collection is empty");
                RuleFor(m => m.Matches).Must(match => match.Count <= batchSettings.Value.Size).WithMessage($"Max batch size of {batchSettings.Value.Size} exceeded");
                RuleForEach(m => m.Matches).ChildRules(c => c.RuleFor(x => x.Companies).NotNull());
                RuleForEach(m => m.Matches).ChildRules(c => c.RuleFor(x => x.Countries).NotNull());
                RuleForEach(m => m.Matches).ChildRules(c => c.RuleFor(x => x.Genres).NotNull());
                RuleForEach(m => m.Matches).ChildRules(c => c.RuleFor(x => x.Languages).NotNull());
            });
        }
    }
}
