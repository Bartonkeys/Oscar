using FluentValidation;
using Microsoft.AspNetCore.Http;
using Oscar.Infrastructure.Features.Matching.Commands;

namespace Oscar.Infrastructure.Features.Matching.Validation
{
    public class MatchCommandValidator : AbstractValidator<MatchCommand>
    {
        public MatchCommandValidator()
        {
        }
    }

    
}
