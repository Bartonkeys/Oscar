using FluentValidation;
using Oscar.Infrastructure.Features.Works.Queries;

namespace Oscar.Infrastructure.Features.Works.Validation;

public class GetWorksByTitleValidator : AbstractValidator<SearchWorksQuery>
{
    public GetWorksByTitleValidator()
    {

    }
}