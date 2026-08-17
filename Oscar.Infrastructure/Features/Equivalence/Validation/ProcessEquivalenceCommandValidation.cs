using FluentValidation;
using Microsoft.AspNetCore.Http;
using Oscar.Infrastructure.Features.Equivalence.Commands;

namespace Oscar.Infrastructure.Features.Equivalence.Validation
{
    public class ProcessEquivalenceCommandValidation : AbstractValidator<ProcessEquivalenceCommand>
    {
        public ProcessEquivalenceCommandValidation()
        {
        }
    }

    
}
