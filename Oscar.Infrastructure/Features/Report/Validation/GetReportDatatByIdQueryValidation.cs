using FluentValidation;
using Oscar.Infrastructure.Features.Report.Queries;

namespace Oscar.Infrastructure.Features.Report.Validation
{
    public class GetReportDatatByIdQueryValidation: AbstractValidator<GetReportDataByIdQuery>
    {
        public GetReportDatatByIdQueryValidation()
        {
            RuleFor(r => r.Id).GreaterThan(0);
        }
    }
}
