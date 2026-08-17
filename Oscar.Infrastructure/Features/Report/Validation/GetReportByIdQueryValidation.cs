using FluentValidation;
using Oscar.Infrastructure.Features.Report.Queries;

namespace Oscar.Infrastructure.Features.Report.Validation
{
    public class GetReportByIdQueryValidation: AbstractValidator<GetReportByIdQuery>
    {
        public GetReportByIdQueryValidation()
        {
            RuleFor(r => r.Id).GreaterThan(0);
        }
    }
}
