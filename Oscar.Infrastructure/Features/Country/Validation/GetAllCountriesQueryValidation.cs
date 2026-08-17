using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Oscar.Infrastructure.Features.Country.Queries;

namespace Oscar.Infrastructure.Features.Country.Validation
{
    public class GetAllCountriesQueryValidation: AbstractValidator<GetAllCountriesQuery>
    {
        public GetAllCountriesQueryValidation()
        {

        }
    }
}
