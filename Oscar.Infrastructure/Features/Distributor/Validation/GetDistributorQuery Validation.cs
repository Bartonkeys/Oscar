using FluentValidation;
using Oscar.Infrastructure.Features.Distributor.Queries;

namespace Oscar.Infrastructure.Features.Distributor.Validation
{
    public class GetDistributorQueryValidation : AbstractValidator<GetDistributorQuery>
    {
        public GetDistributorQueryValidation()
        {
        }
    }
}
