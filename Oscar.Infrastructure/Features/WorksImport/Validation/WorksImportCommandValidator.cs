using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.WorksImport.Commands;

namespace Oscar.Infrastructure.Features.WorksImport.Validation
{
    public class WorksImportCommandValidator: AbstractValidator<WorksImportCommand>
    {
        public WorksImportCommandValidator()
        {
            RuleFor(w => w.WorksImportRequestId).NotEqual(0);
            RuleFor(w => w.Status).NotEqual(WorksImportRequestStatus.None);
        }
    }
}
