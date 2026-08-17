using FluentValidation;
using Oscar.Infrastructure.Features.ProductionCompany.Queries;

namespace Oscar.Infrastructure.Features.ProductionCompany.Validation
{
    public class GetCompanyQueryValidation : AbstractValidator<GetCompanyQuery>
    {
        public GetCompanyQueryValidation()
        {
        }
    }
}
