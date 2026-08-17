

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Oscar.Infrastructure.Features.Clients.Queries;

namespace Oscar.Infrastructure.Features.Clients.Validation
{
    public class GetClientStaticDataQueryValidation : AbstractValidator<GetClientStaticDataQuery>
    {
        public GetClientStaticDataQueryValidation()
        {

        }
    }
}
