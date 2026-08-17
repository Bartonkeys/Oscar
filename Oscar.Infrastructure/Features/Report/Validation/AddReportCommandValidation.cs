using FluentValidation;
using Oscar.Infrastructure.Features.Report.Commands;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Report.Queries;

namespace Oscar.Infrastructure.Features.Report.Validation
{
    public class AddReportCommandValidation : AbstractValidator<AddReportCommand>
    {
        public AddReportCommandValidation()
        {
            RuleFor(r => r.ReportAddDto).NotNull();
        }
    }
    public class AddReportRequestCommandValidation : AbstractValidator<AddReportRequestCommand>
    {
        public AddReportRequestCommandValidation()
        {
            RuleFor(r => r.Id).NotEqual(0);
        }
    }
}