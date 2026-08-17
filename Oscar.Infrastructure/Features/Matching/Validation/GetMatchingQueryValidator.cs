using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Oscar.Infrastructure.Features.Matching.Queries;

namespace Oscar.Infrastructure.Features.Matching.Validation
{
    public class GetMatchingQueryValidator : AbstractValidator<GetMatchingQuery>
    {
        public GetMatchingQueryValidator()
        {

        }
    }
}