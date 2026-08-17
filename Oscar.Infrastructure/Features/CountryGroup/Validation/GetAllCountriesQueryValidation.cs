using FluentValidation;
using Oscar.Infrastructure.Features.Country.Queries;

namespace Oscar.Infrastructure.Features.Country.Validation
{
    public class GetAllCountriesGroupsQueryValidation: AbstractValidator<GetAllCountriesGroupsQuery>
    {
        public GetAllCountriesGroupsQueryValidation()
        {

        }
    }
}
