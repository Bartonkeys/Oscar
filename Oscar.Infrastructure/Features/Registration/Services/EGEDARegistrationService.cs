using AutoMapper;
using BartonKeys.Functional;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Data.Context;
using Oscar.Data.Migrations;
using Oscar.Infrastructure.Features.Registration.Commands;

namespace Oscar.Infrastructure.Features.Registration.Services
{
    public class EGEDARegistrationService : RegistrationService<RegistrationWorksEGEDADto>
    {
        private List<int> _rejectedWorksIds = new List<int>();

        public EGEDARegistrationService(OscarContext oscarContext, IMapper mapper, ILogger<RegistrationService<RegistrationWorksEGEDADto>> logger, IMediator mediator, IServiceScopeFactory serviceScopeFactory) 
            : base(oscarContext, mapper, logger, mediator, serviceScopeFactory)
        {
        }

        public override async Task<Result<RegistrationWorksEGEDADto>> Create(RegistrationBatch registrationBatch, int clientId)
        {
            ClientId = clientId;
            RegistrationBatch = registrationBatch;

            var registrationsResult = await Register(false);

            if (registrationsResult.IsFailure)
                return Result.Fail<RegistrationWorksEGEDADto>(registrationsResult.Error);

            var exports = await ConvertRegistrationsToEGEDA();

            if (exports.Rows.Count() == 0)
                return Result.Fail<RegistrationWorksEGEDADto>(RegistrationError.NoWorks);

            return Result.Ok(exports);
        }

        private async Task<RegistrationWorksEGEDADto> ConvertRegistrationsToEGEDA()
        {
            var registrations = await GetRegistrations();

            var result = new RegistrationWorksEGEDADto()
            {
                FileName = $"{RegistrationBatch.BatchId}/EGEDA_{SanitizeAsFileName(_client.ClientName)}_{DateTime.Now:yyyMMdd}.xlsx",
                ClientName = _client.ClientName,
                Rows = registrations.AsEnumerable().Select(MapEGEDA)
            };
            result.Rows = result.Rows.Where(w => w.CompactRef != "Rejected").ToList();

            if (_rejectedWorksIds.Any())
            {
                await _mediator.Send(new DeleteRegistrationWorksCommand { BatchId = RegistrationBatch.BatchId, WorksIds = _rejectedWorksIds });
            }

            return result;
        }

        private EGEDARow MapEGEDA(Core.Entities.Registration registration)
        {
            if ((registration.Works.Rights == null) | (registration.Works.Rights!.Count == 0))
            {
                registration.Works.Rights = InheritWorksRightsFromParent(registration.Works);
            }
            registration.Works.Rights = registration.Works.Rights.Where(r => (r.Type.Name is "CR" or "BT" or "PP" or "TBOL") && r.Countries.Any(c => c.Code is "ES" or "*")).ToList();

            if (!registration.Works.Rights.Any() || ExemptInES(registration.Works.Rights))
            {
                _rejectedWorksIds.Add(registration.Works.Id);
                return new EGEDARow() { CompactRef = "Rejected" };
            }

            registration.Works.Rights = registration.Works.Rights.Where(r => r.Percentage is not (null or 0)).ToList();

            if (!registration.Works.Rights.Any())
            {
                _rejectedWorksIds.Add(registration.Works.Id);
                return new EGEDARow() { CompactRef = "Rejected" };
            }

            var works = registration.Works;

            var row = _mapper.Map<EGEDARow>(works);

            return row;
        }

        private bool ExemptInES(ICollection<Right> rights)
        {
            bool esCRZeroRights = !rights.Any() ||
                rights.Any(r => r.Type.Name is "CR" && r.Percentage is (null or 0) && r.Countries.Any(c => c.Code is "ES"));

            bool esBTZeroRights = !rights.Any() ||
                rights.Any(r => r.Type.Name is "BT" && r.Percentage is (null or 0) && r.Countries.Any(c => c.Code is "ES"));

            bool esPPZeroRights = !rights.Any() ||
                rights.Any(r => r.Type.Name is "PP" && r.Percentage is (null or 0) && r.Countries.Any(c => c.Code is "ES"));

            bool esTBOLZeroRights = !rights.Any() ||
                rights.Any(r => r.Type.Name is "TBOL" && r.Percentage is (null or 0) && r.Countries.Any(c => c.Code is "ES"));

            var exempted = esCRZeroRights && esBTZeroRights && esPPZeroRights && esTBOLZeroRights;
            return exempted;
        }

        protected async override Task<bool> IsClientRightsValid(Client client)
        {
            var rights = client.Rights.Where(r => r.Type.Name is "CR" or "BT" or "PP" or "TBOL").ToList();
            var isClientRightsValid = !ExemptInES(rights);

            return isClientRightsValid;
        }

    }
}
