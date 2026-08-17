using FluentValidation;
using Oscar.Infrastructure.Features.Document.Queries;

namespace Oscar.Infrastructure.Features.Document.Validation
{
    public class GetDocumentsByTypeQueryValidation : AbstractValidator<GetDocumentsByTypeQuery>
    {
        public GetDocumentsByTypeQueryValidation()
        {
        }
    }
}
