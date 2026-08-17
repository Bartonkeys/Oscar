using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Oscar.Infrastructure.Features.Registration.Queries;

namespace Oscar.Infrastructure.Features.Registration.Validation
{
    public class GetRegistrationBatchQueryValidation : AbstractValidator<GetRegistrationBatchQuery>
    {
        public GetRegistrationBatchQueryValidation()
        {

        }
    }
}
