using FluentValidation;
using Oscar.Infrastructure.Features.Rights.Queries;

namespace Oscar.Infrastructure.Features.Rights.Validation
{

    public class GetMerlinSocietiesQueryValidator : AbstractValidator<GetMerlinSocietiesQuery>
    {
        public GetMerlinSocietiesQueryValidator()
        {
        }
    }
}
