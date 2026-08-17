using FluentValidation;
using Oscar.Infrastructure.Features.Works.Queries;

namespace Oscar.Infrastructure.Features.Works.Validation
{
    public class GetWorksTitleQueryValidation: AbstractValidator<GetWorksTitleQuery>
    {
        public GetWorksTitleQueryValidation()
        {

        }
    }
}
