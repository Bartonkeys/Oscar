using FluentValidation;
using Oscar.Infrastructure.Features.CustomServiceManager.Queries;

namespace Oscar.Infrastructure.Features.Actors.Validation
{
    public class GetAllCustomServiceManagersQueryValidation : AbstractValidator<GetAllCustomServiceManagersQuery>
    {
        public GetAllCustomServiceManagersQueryValidation()
        {

        }
    }

    public class GetAllOperatorsQueryValidation : AbstractValidator<GetAllOperatorsQuery>
    {
        public GetAllOperatorsQueryValidation()
        {

        }
    }
}
