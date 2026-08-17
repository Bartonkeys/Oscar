using FluentValidation;
using Oscar.Infrastructure.Features.Society.Queries;

namespace Oscar.Infrastructure.Features.Society.Validation
{
    public class GetSocietyQueryValidation : AbstractValidator<GetSocietyQuery>
    {
        public GetSocietyQueryValidation()
        {
        }
    }
}
