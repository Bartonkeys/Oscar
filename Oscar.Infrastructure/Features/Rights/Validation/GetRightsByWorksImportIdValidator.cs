using FluentValidation;
using Oscar.Infrastructure.Features.Rights.Queries;

namespace Oscar.Infrastructure.Features.Rights.Validation
{

    public class GetRightsByWorksImportIdValidator : AbstractValidator<GetRightsByWorksImportIdQuery>
    {
        public GetRightsByWorksImportIdValidator()
        {
            RuleFor(r => r.WorksImportId).GreaterThan(0);
        }
    }
}
