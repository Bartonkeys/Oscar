using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Oscar.Infrastructure.Features.Report.Queries;

namespace Oscar.Infrastructure.Features.Report.Validation
{
    public class GetReportBaseEntitiesValidation : AbstractValidator<GetReportBaseEntities>
    {
        public GetReportBaseEntitiesValidation()
        {

        }
    }
}
