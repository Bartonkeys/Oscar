using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Oscar.Infrastructure.Features.Clients.Queries;

namespace Oscar.Infrastructure.Features.Actor.Validation
{
    public class GetAllClientsQueryValidation: AbstractValidator<GetAllClientsQuery>
    {
        public GetAllClientsQueryValidation()
        {

        }
    }
}
