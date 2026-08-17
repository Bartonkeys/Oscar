using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Oscar.Infrastructure.Features.Producer.Queries;

namespace Oscar.Infrastructure.Features.Producer.Validation
{
    public class GetAllProducersQueryValidation : AbstractValidator<GetAllProducersQuery>
    {
        public GetAllProducersQueryValidation()
        {

        }
    }
}
