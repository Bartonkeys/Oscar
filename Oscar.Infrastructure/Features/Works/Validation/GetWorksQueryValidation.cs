using FluentValidation;
using Oscar.Infrastructure.Features.Works.Queries;

namespace Oscar.Infrastructure.Features.Works.Validation
{
    public class GetWorksQueryValidation: AbstractValidator<GetWorksQuery>
    {
        public GetWorksQueryValidation()
        {

        }
    }
}
