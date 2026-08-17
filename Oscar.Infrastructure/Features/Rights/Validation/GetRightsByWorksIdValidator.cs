using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Oscar.Infrastructure.Features.Episode.Queries;
using Oscar.Infrastructure.Features.Rights.Queries;

namespace Oscar.Infrastructure.Features.Rights.Validation
{

    public class GetRightsByWorksIdValidator : AbstractValidator<GetRightsByWorksIdQuery>
    {
        public GetRightsByWorksIdValidator()
        {
            RuleFor(r => r.WorksId).GreaterThan(0);
        }
    }
}
