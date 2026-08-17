using FluentValidation;
using Oscar.Infrastructure.Features.Report.Commands;
using Oscar.Core.DTOs;
using Oscar.Data.Context;

namespace Oscar.Infrastructure.Features.Report.Validation
{
    public class EditReportCommandValidation : AbstractValidator<EditReportCommand>
    {
        public EditReportCommandValidation()
        {
            RuleFor(r => r.ReportEditDto).NotNull();
        }
    }
}