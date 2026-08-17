using FluentValidation;
using Oscar.Infrastructure.Features.Works.Queries;

namespace Oscar.Infrastructure.Features.Works.Validation;

public class SearchByTitleQueryValidator : AbstractValidator<SearchByTitleQuery>
{
    public SearchByTitleQueryValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty();
    }
}