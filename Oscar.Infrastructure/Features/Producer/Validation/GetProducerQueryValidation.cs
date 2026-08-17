using FluentValidation;
using Oscar.Infrastructure.Features.Producer.Queries;

namespace Oscar.Infrastructure.Features.Producer.Validation
{
    public class GetProducerQueryValidation : AbstractValidator<GetProducerQuery>
    {
        public GetProducerQueryValidation()
        {
        }
    }
}
