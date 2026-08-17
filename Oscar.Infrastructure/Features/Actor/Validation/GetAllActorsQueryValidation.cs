using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Oscar.Infrastructure.Features.Actor.Queries;

namespace Oscar.Infrastructure.Features.Actors.Validation
{
    public class GetAllActorsQueryValidation: AbstractValidator<GetAllActorsQuery>
    {
        public GetAllActorsQueryValidation()
        {

        }
    }
}
