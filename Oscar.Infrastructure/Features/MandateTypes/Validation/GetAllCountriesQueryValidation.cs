using FluentValidation;
using Oscar.Infrastructure.Features.MandateTypes.Queries;

namespace Oscar.Infrastructure.Features.MandateTypes.Validation
{
    public class GetAllMandateTypesQueryValidation : AbstractValidator<GetAllMandateTypesQuery>
    {
        public GetAllMandateTypesQueryValidation()
        {

        }
    }
}
