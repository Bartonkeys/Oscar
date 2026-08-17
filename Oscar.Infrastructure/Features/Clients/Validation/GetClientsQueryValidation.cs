using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Oscar.Infrastructure.Features.Clients.Queries;

namespace Oscar.Infrastructure.Features.Clients.Validation
{
    public class GetClientsQueryValidation: AbstractValidator<GetClientsQuery>
    {
        public GetClientsQueryValidation()
        {

        }
    }
}
