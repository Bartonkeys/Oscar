using FluentValidation;
using Oscar.Infrastructure.Features.ScriptWriter.Queries;

namespace Oscar.Infrastructure.Features.ScriptWriter.Validation
{
    public class GetScriptWriterQueryValidation : AbstractValidator<GetScriptWriterQuery>
    {
        public GetScriptWriterQueryValidation()
        {
        }
    }
}
