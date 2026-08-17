using FluentValidation;
using Oscar.Infrastructure.Features.Series.Queries;

namespace Oscar.Infrastructure.Features.Works.Validation;

public class GetAllWorksByClientQueryValidation : AbstractValidator<GetWorksByClientQuery>
{
    public GetAllWorksByClientQueryValidation()
    {
    }
}