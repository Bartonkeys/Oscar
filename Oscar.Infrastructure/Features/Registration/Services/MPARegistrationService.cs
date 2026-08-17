using AutoMapper;
using BartonKeys.Functional;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Registration.Commands;

namespace Oscar.Infrastructure.Features.Registration.Services
{
    public class MPARegistrationService : RegistrationService<RegistrationWorksMPADto>
    {
        private List<int> _rejectedWorksIds = new List<int>();

        public MPARegistrationService(OscarContext oscarContext, IMapper mapper, ILogger<RegistrationService<RegistrationWorksMPADto>> logger, IMediator mediator, IServiceScopeFactory serviceScopeFactory) 
            : base(oscarContext, mapper, logger, mediator, serviceScopeFactory)
        {
        }

        public override async Task<Result<RegistrationWorksMPADto>> Create(RegistrationBatch registrationBatch, int clientId)
        {
            ClientId = clientId;
            RegistrationBatch = registrationBatch;

            var registrationsResult = await Register();

            if (registrationsResult.IsFailure)
                return Result.Fail<RegistrationWorksMPADto>(registrationsResult.Error);

            var exports = await ConvertRegistrationsToMPA();

            return Result.Ok(exports);
        }

        private async Task<RegistrationWorksMPADto> ConvertRegistrationsToMPA()
        {
            var registrations = await GetRegistrations();

            var result = new RegistrationWorksMPADto()
            {
                FileName = $"{RegistrationBatch.BatchId}/MPA_{SanitizeAsFileName(_client.ClientName)}_{DateTime.Now:yyyMMdd}.xlsx",
                ClientName = _client.ClientName,
                Rows = registrations.AsEnumerable().Select(MapMPA)
            };
            result.Rows = result.Rows.Where(w => w.ClaimantId != "Rejected").ToList();

            if (_rejectedWorksIds.Any())
            {
                await _mediator.Send(new DeleteRegistrationWorksCommand { BatchId = RegistrationBatch.BatchId, WorksIds = _rejectedWorksIds });
            }

            return result;
        }

        private MPARow MapMPA(Core.Entities.Registration registration)
        {
            if ((registration.Works.Rights == null) | (registration.Works.Rights!.Count == 0))
            {
                registration.Works.Rights = InheritWorksRightsFromParent(registration.Works);
            }

            registration.Works.Rights = registration.Works.Rights.Where(r => r.Type.Name is "CR" && r.Countries.Any(c => c.Code is "US" or "*")).ToList();

            if (!registration.Works.Rights.Any() || ExemptInUS(registration.Works.Rights))
            {
                _rejectedWorksIds.Add(registration.Works.Id);
                return new MPARow() { ClaimantId = "Rejected" };
            }

            registration.Works.Rights = registration.Works.Rights.Where(r => r.Percentage is not (null or 0)).ToList();

            if (!registration.Works.Rights.Any())
            {
                _rejectedWorksIds.Add(registration.Works.Id);
                return new MPARow() { ClaimantId = "Rejected" };
            }

            var works = registration.Works;

            var row = _mapper.Map<MPARow>(works);

            row.ClaimantId = _client.ClientReference?.ToString();

            return row;
        }

        private static bool ExemptInUS(ICollection<Right> rights)
        {
            return rights.Any(r => r.Percentage is (null or 0) && r.Countries.Any(c => c.Code is "US"));
        }

        protected override async Task<bool> IsClientRightsValid(Client client)
        {
            var rights = client.Rights.Where(r => r.Type.Name is "CR").ToList();
            var isClientRightsValid = !ExemptInUS(rights);

            return isClientRightsValid;
        }

    }
}
