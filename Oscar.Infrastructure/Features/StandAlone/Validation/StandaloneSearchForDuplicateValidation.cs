using FluentValidation;
using Oscar.Infrastructure.Features.StandAlone.Queries;

namespace Oscar.Infrastructure.Features.StandAlone.Validation
{
    public class StandAloneSearchForDuplicateValidation: AbstractValidator<StandAloneSearchForDuplicate>
    {
        public StandAloneSearchForDuplicateValidation()
        {
        }
    }
}
