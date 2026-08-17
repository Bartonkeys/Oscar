using FluentValidation;
using Oscar.Infrastructure.Features.Series.Queries;

namespace Oscar.Infrastructure.Features.Series.Validation;

public class GetAllSeriesQueryValidation : AbstractValidator<GetAllSeriesQuery>
{
    public GetAllSeriesQueryValidation()
    {
    }
}