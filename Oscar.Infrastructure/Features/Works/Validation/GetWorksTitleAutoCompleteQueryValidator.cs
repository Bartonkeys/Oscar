using FluentValidation;
using Oscar.Infrastructure.Features.Works.Queries;

namespace Oscar.Infrastructure.Features.Works.Validation;

public class GetWorksTitleAutoCompleteQueryValidator : AbstractValidator<GetWorksTitleAutoCompleteQuery>
{
    public GetWorksTitleAutoCompleteQueryValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty();
    }
}