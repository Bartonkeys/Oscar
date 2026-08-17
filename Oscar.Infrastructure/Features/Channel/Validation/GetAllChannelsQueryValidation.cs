using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Oscar.Infrastructure.Features.Channel.Queries;

namespace Oscar.Infrastructure.Features.Channel.Validation
{
    public class GetAllChannelsQueryValidation: AbstractValidator<GetAllChannelsQuery>
    {
        public GetAllChannelsQueryValidation()
        {

        }
    }
}
