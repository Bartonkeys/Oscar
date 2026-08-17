using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Oscar.Infrastructure.Features.ProductionCompany.Queries;

namespace Oscar.Infrastructure.Features.ProductionCompany.Validation
{
    public class GetAllCompaniesQueryValidation: AbstractValidator<GetAllCompaniesQuery>
    {
        public GetAllCompaniesQueryValidation()
        {

        }
    }
}
