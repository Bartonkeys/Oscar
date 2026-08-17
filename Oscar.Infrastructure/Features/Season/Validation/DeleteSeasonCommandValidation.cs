using FluentValidation;
using Oscar.Infrastructure.Features.Season.Commands;

namespace Oscar.Infrastructure.Features.Season.Validation
{
    public class DeleteSeasonCommandValidation: AbstractValidator<DeleteSeasonCommand>
    {

        public DeleteSeasonCommandValidation()
        {
            RuleFor(r => r.Id).NotEqual(0);
        }
    }
}
