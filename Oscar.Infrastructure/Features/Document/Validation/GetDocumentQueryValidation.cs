using FluentValidation;
using Oscar.Infrastructure.Features.Document.Queries;

namespace Oscar.Infrastructure.Features.Document.Validation
{
    public class GetDocumentQueryValidation : AbstractValidator<GetDocumentQuery>
    {
        public GetDocumentQueryValidation()
        {
        }
    }
}
