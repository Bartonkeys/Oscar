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
    public class CRCRegistrationService : RegistrationService<RegistrationWorksCRCDto>
    {
        private List<int> _rejectedWorksIds = new List<int>();

        public CRCRegistrationService(OscarContext oscarContext, IMapper mapper, ILogger<RegistrationService<RegistrationWorksCRCDto>> logger, IMediator mediator, IServiceScopeFactory serviceScopeFactory) 
            : base(oscarContext, mapper, logger, mediator, serviceScopeFactory)
        {
        }

        public override async Task<Result<RegistrationWorksCRCDto>> Create(RegistrationBatch registrationBatch, int clientId)
        {
            ClientId = clientId;
            RegistrationBatch = registrationBatch;

            var registrationsResult = await Register();

            if (registrationsResult.IsFailure)
                return Result.Fail<RegistrationWorksCRCDto>(registrationsResult.Error);

            var screenrightsExports = await ConvertRegistrationsToCMC();

            return Result.Ok(screenrightsExports);
        }

        private async Task<RegistrationWorksCRCDto> ConvertRegistrationsToCMC()
        {
            var registrations = await GetRegistrations();

            var result = new RegistrationWorksCRCDto()
            {
                FileName = $"{RegistrationBatch.BatchId}/CRC_{SanitizeAsFileName(_client.ClientName)}_{DateTime.Now:yyyMMdd}.xlsx",
                ClientName = _client.ClientName,
                Rows = registrations.AsEnumerable().Select(MapCRC)
            };
            result.Rows = result.Rows.Where(w => w.CompactRef != "Rejected").ToList();

            if (_rejectedWorksIds.Any())
            {
                await _mediator.Send(new DeleteRegistrationWorksCommand { BatchId = RegistrationBatch.BatchId, WorksIds = _rejectedWorksIds });
            }

            return result;
        }

        private CRCRow MapCRC(Core.Entities.Registration registration)
        {

            if ((registration.Works.Rights == null) | (registration.Works.Rights!.Count == 0))
            {
                registration.Works.Rights = InheritWorksRightsFromParent(registration.Works);
            }
            
            registration.Works.Rights = registration.Works.Rights.Where(r => r.Type.Name == "CR" && r.Countries.Any(c => c.Code is "CA" or "*")).ToList();

            if (!registration.Works.Rights.Any() || ExemptInCA(registration.Works.Rights))
            {
                _rejectedWorksIds.Add(registration.Works.Id);
                return new CRCRow() { CompactRef = "Rejected" };
            }

            registration.Works.Rights = registration.Works.Rights.Where(r => r.Percentage is not (null or 0)).ToList();

            if (!registration.Works.Rights.Any())
            {
                _rejectedWorksIds.Add(registration.Works.Id);
                return new CRCRow() { CompactRef = "Rejected" };
            }

            var works = registration.Works;

            var row = _mapper.Map<CRCRow>(works);

            row.Name = _client.ClientName;

            return row;
        }

        private static bool ExemptInCA(ICollection<Right> rights)
        {
            return rights.Any(r => r.Percentage is (null or 0) && r.Countries.Any(c => c.Code is "CA"));
        }

        protected override async Task<bool> IsClientRightsValid(Client client)
        {
            var rights = client.Rights.Where(r => r.Type.Name is "CR").ToList();
            var isClientRightsValid = !ExemptInCA(rights);

            return isClientRightsValid;
        }

    }
}
