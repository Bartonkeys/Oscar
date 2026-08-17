using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Oscar.Infrastructure.Features.Distributor.Queries;

namespace Oscar.Infrastructure.Features.Distributor.Validation
{
    public class GetAllDistributorsQueryValidation : AbstractValidator<GetAllDistributorsQuery>
    {
        public GetAllDistributorsQueryValidation()
        {

        }
    }
}
