using FluentValidation;
using Oscar.Infrastructure.Features.Country.Queries;

namespace Oscar.Infrastructure.Features.Country.Validation
{
    public class GetCountryQueryValidation : AbstractValidator<GetCountryQuery>
    {
        public GetCountryQueryValidation()
        {
        }
    }

    public class GetCountryByIdQueryValidation : AbstractValidator<GetCountryByIdQuery>
    {
        public GetCountryByIdQueryValidation()
        {
        }
    }
}
