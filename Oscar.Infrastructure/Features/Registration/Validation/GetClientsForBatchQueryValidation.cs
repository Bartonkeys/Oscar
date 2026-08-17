using FluentValidation;
using Oscar.Infrastructure.Features.Registration.Queries;

namespace Oscar.Infrastructure.Features.Registration.Validation;

public class GetClientsForBatchQueryValidation : AbstractValidator<GetClientsForBatchQuery>
{
    public GetClientsForBatchQueryValidation()
    {
        RuleFor(q => q.BatchId).NotEqual(Guid.Empty);
    }
}