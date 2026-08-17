using FluentValidation;
using Oscar.Infrastructure.Features.ScreenWriter.Queries;

namespace Oscar.Infrastructure.Features.ScreenWriter.Validation
{
    public class GetScreenWriterQueryValidation : AbstractValidator<GetScreenWriterQuery>
    {
        public GetScreenWriterQueryValidation()
        {
        }
    }
}
