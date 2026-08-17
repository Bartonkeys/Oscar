using BartonKeys.Functional;
using Oscar.Core.Entities;

namespace Oscar.Infrastructure.Features.Registration.Contracts;

public interface IRegistrationService<T>
{
    Task<Result<T>> Create(RegistrationBatch registrationBatch, int clientId);
}