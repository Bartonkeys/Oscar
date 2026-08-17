using FluentValidation;
using Oscar.Infrastructure.Features.Series.Queries;

namespace Oscar.Infrastructure.Features.Series.Validation;

public class SeriesSearchByTitleQueryValidation : AbstractValidator<SeriesSearchByTitleQuery>
{
    public SeriesSearchByTitleQueryValidation()
    {
        RuleFor(r => r.Title).NotNull().NotEmpty();
    }
}