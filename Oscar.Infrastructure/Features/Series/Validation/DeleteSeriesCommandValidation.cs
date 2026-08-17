using FluentValidation;
using Oscar.Infrastructure.Features.Series.Commands;

namespace Oscar.Infrastructure.Features.Series.Validation
{
    public class DeleteSeriesCommandValidation: AbstractValidator<DeleteSeriesCommand>
    {

        public DeleteSeriesCommandValidation()
        {
            RuleFor(r => r.Id).NotEqual(0);
        }
    }
}
