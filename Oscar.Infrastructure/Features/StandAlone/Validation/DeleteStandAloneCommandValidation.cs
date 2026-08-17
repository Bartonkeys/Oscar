using FluentValidation;
using Oscar.Infrastructure.Features.StandAlone.Commands;

namespace Oscar.Infrastructure.Features.StandAlone.Validation
{
    public class DeleteStandAloneCommandValidation: AbstractValidator<DeleteStandAloneCommand>
    {

        public DeleteStandAloneCommandValidation()
        {
            RuleFor(r => r.Id).NotEqual(0);
        }
    }
}
