using FluentValidation;
using Microsoft.AspNetCore.Http;
using Oscar.Infrastructure.Features.Screenrights.Commands;

namespace Oscar.Infrastructure.Features.Screenrights.Validation
{
    public class ProcessScreenrightsCommandValidation : AbstractValidator<ProcessScreenrightsCommand>
    {
        public ProcessScreenrightsCommandValidation()
        {
        }
    }

    
}
