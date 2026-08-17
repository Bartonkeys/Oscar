using AutoMapper;
using BartonKeys.Functional;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Registration.Commands;

namespace Oscar.Infrastructure.Features.Registration.Services
{
    public class UpfarArgoaRegistrationService : RegistrationService<RegistrationWorksUpfarArgoaDto>
    {
        private List<int> _rejectedWorksIds = new List<int>();

        public UpfarArgoaRegistrationService(OscarContext oscarContext, IMapper mapper, ILogger<RegistrationService<RegistrationWorksUpfarArgoaDto>> logger, IMediator mediator, IServiceScopeFactory serviceScopeFactory)
            : base(oscarContext, mapper, logger, mediator, serviceScopeFactory)
        {
        }

        public override async Task<Result<RegistrationWorksUpfarArgoaDto>> Create(RegistrationBatch registrationBatch, int clientId)
        {
            ClientId = clientId;
            RegistrationBatch = registrationBatch;

            var registrationsResult = await Register();

            if (registrationsResult.IsFailure)
                return Result.Fail<RegistrationWorksUpfarArgoaDto>(registrationsResult.Error);

            var exports = await ConvertRegistrationsToUpfarArgoa();

            return Result.Ok(exports);
        }

        private async Task<RegistrationWorksUpfarArgoaDto> ConvertRegistrationsToUpfarArgoa()
        {
            var registrations = await GetRegistrations();

            var result = new RegistrationWorksUpfarArgoaDto()
            {
                FileName = $"{RegistrationBatch.BatchId}/Upfar_Argoa_{SanitizeAsFileName(_client.ClientName)}_{DateTime.Now:yyyMMdd}.xlsx",
                Rows = registrations.AsEnumerable().Select(MapUpfarArgoa)
            };
            result.Rows = result.Rows.Where(w => w.SeriesOrStandAloneTitle != "Rejected").ToList();

            if (_rejectedWorksIds.Any())
            {
                await _mediator.Send(new DeleteRegistrationWorksCommand { BatchId = RegistrationBatch.BatchId, WorksIds = _rejectedWorksIds });
            }

            return result;
        }

        private UpfarArgoaRow MapUpfarArgoa(Core.Entities.Registration registration)
        {
            if ((registration.Works.Rights == null) | (registration.Works.Rights!.Count == 0))
            {
                registration.Works.Rights = InheritWorksRightsFromParent(registration.Works);
            }
            registration.Works.Rights = registration.Works.Rights.Where(r => r.Type.Name is "BT" && r.Countries.Any(c => c.Code is "RO" or "*")).ToList();

            if (!registration.Works.Rights.Any() || ExemptInRO(registration.Works.Rights))
            {
                _rejectedWorksIds.Add(registration.Works.Id);
                return new UpfarArgoaRow() { SeriesOrStandAloneTitle = "Rejected" };
            }

            registration.Works.Rights = registration.Works.Rights.Where(r => r.Percentage is not (null or 0)).ToList();

            if (!registration.Works.Rights.Any())
            {
                _rejectedWorksIds.Add(registration.Works.Id);
                return new UpfarArgoaRow() { SeriesOrStandAloneTitle = "Rejected" };
            }

            var works = registration.Works;

            var row = _mapper.Map<UpfarArgoaRow>(works);

            var specificRight = registration.Works.Rights.FirstOrDefault(r => r.Countries.Any(c => c.Code == "RO") && r.Percentage > 0);

            row.QuotaRightsHeld = specificRight != null ? specificRight.Percentage.ToString() : registration.Works.Rights.FirstOrDefault(r => r.Countries.Any(c => c.Code == "*") && r.Percentage > 0)?.Percentage.ToString();

            if (works.DurationMinutes != null)
            {
                var formattedDuration = TimeSpan.FromMinutes(works.DurationMinutes.Value);
                row.Duration = formattedDuration.ToString("c");
            }

            row.RightHolder = _client.ClientName;
            row.ManagedRights = "7,9";
            row.DateOfRegistration = DateTime.Now.ToString("yyyy-MM-dd");
            row.Observations = "Romania";
            row.ReciprocalContracts = "COMPACT COLLECTIONS 01/05.01.2015";
            row.RightsFrom = registration.Works.Rights.FirstOrDefault()?.StartOfRight.ToString("yyyy-MM-dd");
            row.RightsTo = registration.Works.Rights.FirstOrDefault()?.EndOfRight.ToString("yyyy-MM-dd");

            return row;
        }

        private static bool ExemptInRO(ICollection<Right> rights)
        {
            return rights.Any(r => r.Percentage is (null or 0) && r.Countries.Any(c => c.Code is "RO"));
        }

        protected async override Task<bool> IsClientRightsValid(Client client)
        {
            var rights = client.Rights.Where(r => r.Type.Name is "BT").ToList();
            var isClientRightsValid = !ExemptInRO(rights);

            return isClientRightsValid;
        }

    }
}
