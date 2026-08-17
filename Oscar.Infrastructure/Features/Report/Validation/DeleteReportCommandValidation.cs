using FluentValidation;
using Oscar.Infrastructure.Features.Report.Commands;

namespace Oscar.Infrastructure.Features.Report.Validation
{
    public class DeleteReportCommandValidation: AbstractValidator<DeleteReportCommand>
    {

        public DeleteReportCommandValidation()
        {
            RuleFor(r => r.Id).NotEqual(0);
        }
    }
}
