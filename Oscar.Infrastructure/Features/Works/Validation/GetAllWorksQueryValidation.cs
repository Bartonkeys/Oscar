using FluentValidation;
using Oscar.Infrastructure.Features.Series.Queries;

namespace Oscar.Infrastructure.Features.Works.Validation;

public class GetAllWorksQueryValidation : AbstractValidator<GetAllWorksQuery>
{
    public GetAllWorksQueryValidation()
    {
    }
}