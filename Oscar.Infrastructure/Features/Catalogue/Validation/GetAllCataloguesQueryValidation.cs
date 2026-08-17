using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Oscar.Infrastructure.Features.Catalogue.Queries;

namespace Oscar.Infrastructure.Features.Actor.Validation
{
    public class GetAllCataloguesQueryValidation: AbstractValidator<GetAllCataloguesQuery>
    {
        public GetAllCataloguesQueryValidation()
        {

        }
    }
}
