using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Oscar.Infrastructure.Features.Director.Queries;

namespace Oscar.Infrastructure.Features.Director.Validation
{
    public class GetAllDirectorsQueryValidation: AbstractValidator<GetAllDirectorsQuery>
    {
        public GetAllDirectorsQueryValidation()
        {

        }
    }
}
