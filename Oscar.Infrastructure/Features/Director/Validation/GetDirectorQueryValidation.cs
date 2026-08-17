using FluentValidation;
using Oscar.Infrastructure.Features.Director.Queries;

namespace Oscar.Infrastructure.Features.Director.Validation
{
    public class GetDirectorQueryValidation : AbstractValidator<GetDirectorQuery>
    {
        public GetDirectorQueryValidation()
        {
        }
    }
}
