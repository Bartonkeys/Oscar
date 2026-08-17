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
    public class CCCRegistrationService : RegistrationService<RegistrationWorksCCCDto>
    {
        private List<int> _rejectedWorksIds = new List<int>();

        public CCCRegistrationService(OscarContext oscarContext, IMapper mapper, ILogger<RegistrationService<RegistrationWorksCCCDto>> logger, IMediator mediator, IServiceScopeFactory serviceScopeFactory) 
            : base(oscarContext, mapper, logger, mediator, serviceScopeFactory)
        {
        }

        public override async Task<Result<RegistrationWorksCCCDto>> Create(RegistrationBatch registrationBatch, int clientId)
        {
            ClientId = clientId;
            RegistrationBatch = registrationBatch;

            var registrationsResult = await Register();

            if (registrationsResult.IsFailure)
                return Result.Fail< RegistrationWorksCCCDto>(registrationsResult.Error);

            var cccExports = await ConvertRegistrationsToCCC();

            return Result.Ok(cccExports);
        }

        private async Task<RegistrationWorksCCCDto> ConvertRegistrationsToCCC()
        {
            var registrations = await GetRegistrations();

            var result = new RegistrationWorksCCCDto()
            {
                FileName = $"{RegistrationBatch.BatchId}/CCC_{_client.ClientReference}_{SanitizeAsFileName(_client.ClientName)}_{DateTime.Now:yyyMMdd}.xlsm",
                ClaimantName = $"CLAIMANT NAME: {_client.ClientName} ({_client.ClientReference})",
                RoyaltyPeriod = $"For Royalty Period January 1 to December 31, {DateTime.Now.Year}",
                ReturnDate = $"RETURN DATE: {DateTime.Now.ToLongDateString()}",
                CccHeader = new()
                {
                    Year = DateTime.Now.Year.ToString()
                },
                Rows = registrations.AsEnumerable().Select(MapCCC)
            };
            result.Rows = result.Rows.Where(w => w.ClaimantInternalReferenceNumber != "Rejected").ToList();

            if (_rejectedWorksIds.Any())
            {
                await _mediator.Send(new DeleteRegistrationWorksCommand { BatchId = RegistrationBatch.BatchId, WorksIds = _rejectedWorksIds });
            }

            return result;
        }

        private CCCRow MapCCC(Core.Entities.Registration registration)
        {
            if (registration.Works.Rights!.Count == 0)
            {
                registration.Works.Rights = InheritWorksRightsFromParent(registration.Works);
            }

            registration.Works.Rights = registration.Works.Rights.Where(r => r.Type.Name == "CR" && r.Countries.Any(c => c.Code is "CA" or "*")).ToList();

            if (!registration.Works.Rights.Any() || ExemptInCA(registration.Works.Rights))
            {
                _rejectedWorksIds.Add(registration.Works.Id);
                return new CCCRow() { ClaimantInternalReferenceNumber = "Rejected" };
            }

            registration.Works.Rights = registration.Works.Rights.Where(r => r.Percentage is not (null or 0)).ToList();

            if (!registration.Works.Rights.Any())
            {
                _rejectedWorksIds.Add(registration.Works.Id);
                return new CCCRow() { ClaimantInternalReferenceNumber = "Rejected" };
            }

            var row = _mapper.Map<CCCRow>(registration);

            row.ClaimantId = _client.ClientReference?.ToString();

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

        protected override bool IsSocietyRightsClaimableOnWork(Core.Entities.Works works, Core.Entities.Society society)
        {
            var isWorkProducedinUSA = (works?.Countries?.Where(x => x.Code == "US").Count() != 0);
            return isWorkProducedinUSA;
        }

    }
}
