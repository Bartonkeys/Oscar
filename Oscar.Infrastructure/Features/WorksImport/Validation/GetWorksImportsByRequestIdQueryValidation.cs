using FluentValidation;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.WorksImport.Queries;

namespace Oscar.Infrastructure.Features.WorksImport.Validation
{
    public class GetWorksImportsByRequestIdQueryValidation : AbstractValidator<GetWorksImportsByRequestIdQuery>
    {
        public GetWorksImportsByRequestIdQueryValidation()
        {
            RuleFor(r => r.Start)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Start must greater than or equal to 0");
        }
    }
}
