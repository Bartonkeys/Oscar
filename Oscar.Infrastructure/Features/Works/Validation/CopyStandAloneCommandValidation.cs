using FluentValidation;
using Oscar.Infrastructure.Features.Works.Commands;
using Oscar.Core.DTOs;

namespace Oscar.Infrastructure.Features.Works.Validation
{
    public class CopyStandAloneCommandValidation : AbstractValidator<CopyStandAloneCommand>
    {
        public CopyStandAloneCommandValidation()
        {
        }
    }
}