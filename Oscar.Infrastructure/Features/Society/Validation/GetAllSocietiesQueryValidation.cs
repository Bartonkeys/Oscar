using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Oscar.Infrastructure.Features.Society.Queries;

namespace Oscar.Infrastructure.Features.Society.Validation
{
    public class GetAllSocietiesQueryValidation : AbstractValidator<GetAllSocietiesQuery>
    {
        public GetAllSocietiesQueryValidation()
        {

        }
    }
}

