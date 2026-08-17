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
    public class MPLCRegistrationService : RegistrationService<RegistrationWorksMPLCDto>
    {
        private List<int> _rejectedWorksIds = new List<int>();

        public MPLCRegistrationService(OscarContext oscarContext, IMapper mapper, ILogger<RegistrationService<RegistrationWorksMPLCDto>> logger, IMediator mediator, IServiceScopeFactory serviceScopeFactory) 
            : base(oscarContext, mapper, logger, mediator, serviceScopeFactory)
        {
        }

        public override async Task<Result<RegistrationWorksMPLCDto>> Create(RegistrationBatch registrationBatch, int clientId)
        {
            ClientId = clientId;
            RegistrationBatch = registrationBatch;

            var registrationsResult = await Register();

            if (registrationsResult.IsFailure)
                return Result.Fail<RegistrationWorksMPLCDto>(registrationsResult.Error);

            var results = await ConvertRegistrationsToMPLC();

            return Result.Ok(results);
        }

        private async Task<RegistrationWorksMPLCDto> ConvertRegistrationsToMPLC()
        {
            await AddRegisteredWorksMissingClientReferences();
            var registrations = await GetRegistrations();

            var result = new RegistrationWorksMPLCDto()
            {
                FileName = $"{RegistrationBatch.BatchId}/MPLC_{_client.ClientReference}_{SanitizeAsFileName(_client.ClientName)}_{DateTime.Now:yyyMMdd}.xlsx",
                Rows = registrations.AsEnumerable().Select(MapMPLC)
            };
            result.Rows = result.Rows.Where(w => w.CompactRef != "Rejected").ToList();

            if (_rejectedWorksIds.Any())
            {
                await _mediator.Send(new DeleteRegistrationWorksCommand { BatchId = RegistrationBatch.BatchId, WorksIds = _rejectedWorksIds });
            }

            return result;
        }

        private MPLCRow MapMPLC(Core.Entities.Registration registration)
        {
            if (registration.Works.Rights!.Count == 0)
            {
                registration.Works.Rights = InheritWorksRightsFromParent(registration.Works);
            }

            registration.Works.Rights = registration.Works.Rights.Where(r => r.Type.Name is "PP" && r.Percentage is not (null or 0) && r.EndOfRight > DateTime.Now).ToList();

            if (!registration.Works.Rights.Any())
            {
                _rejectedWorksIds.Add(registration.Works.Id);
                return new MPLCRow() { CompactRef = "Rejected" };
            }

            var works = registration.Works;

            var row = _mapper.Map<MPLCRow>(works);

            return row;
        }

        protected override async Task<bool> IsClientRightsValid(Client client)
        {
            client.Rights = client.Rights.Where(r => r.Type.Name is "PP").ToList();

            if (client.Rights.Any(worksRight => worksRight.Countries.Any(c => c.Name == "UNITED KINGDOM") && worksRight.Percentage == 0))
                return false;

            if (client.Rights.Any(worksRight => worksRight.Countries.Any(c => c.Name == "WORLD") && worksRight.Percentage > 0))
                return true;

            if (client.Rights.Any(worksRight => worksRight.Countries.Any(c => c.Name == "UNITED KINGDOM") && worksRight.Percentage > 0))
                return true;

            return false;
        }

        protected override bool IsSocietyRightsClaimableOnWork(Core.Entities.Works works, Core.Entities.Society society)
        {
            return true;
        }

        protected override bool IsValidWorkRightsForSocietyTerritory(Core.Entities.Works works, Core.Entities.Society society)
        {
            return IsValidWorkRights(works);
        }

        private bool IsValidWorkRights(Core.Entities.Works works)
        {
            if ((works.Rights == null) | (works.Rights!.Count == 0))
            {
                works.Rights = InheritWorksRightsFromParent(works);
            }

            works.Rights = works.Rights.Where(r => r.Type.Name is "PP").ToList();

            return !works.Rights.Any(worksRight =>
                       worksRight.Countries.Any(c => c.Name == "UNITED KINGDOM") && worksRight.Percentage is (null or 0)) &&
                   (works.Rights.Any(worksRight =>
                       worksRight.Countries.Any(c => c.Name == "WORLD") && worksRight.Percentage > 0) ||
                   works.Rights.Any(
                       worksRight => worksRight.Countries.Any(c => c.Name == "UNITED KINGDOM") && worksRight.Percentage > 0));
        }
    }
}
