using FluentValidation;
using Oscar.Infrastructure.Features.WorksImport.Queries;

namespace Oscar.Infrastructure.Features.WorksImport.Validation
{
    public class GetWorksImportRequestByIdQueryValidation: AbstractValidator<GetWorksImportRequestByIdQuery>
    {
        public GetWorksImportRequestByIdQueryValidation()
        {
            RuleFor(r => r.Id).GreaterThan(0);
        }
    }
}
