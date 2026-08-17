using AutoMapper;
using BartonKeys.Functional;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Data.Migrations;
using Oscar.Infrastructure.Features.Registration.Commands;

namespace Oscar.Infrastructure.Features.Registration.Services
{
    public class GWFFRegistrationService : RegistrationService<RegistrationWorksGWFFDto>
    {
        private List<int> _rejectedWorksIds = new List<int>();

        public GWFFRegistrationService(OscarContext oscarContext, IMapper mapper, ILogger<RegistrationService<RegistrationWorksGWFFDto>> logger, IMediator mediator, IServiceScopeFactory serviceScopeFactory) 
            : base(oscarContext, mapper, logger, mediator, serviceScopeFactory)
        {
        }

        public override async Task<Result<RegistrationWorksGWFFDto>> Create(RegistrationBatch registrationBatch, int clientId)
        {
            ClientId = clientId;
            RegistrationBatch = registrationBatch;

            var registrationsResult = await Register(false);

            if (registrationsResult.IsFailure)
                return Result.Fail<RegistrationWorksGWFFDto>(registrationsResult.Error);

            var exports = await ConvertRegistrationsToEGEDA();

            if (exports.Rows.Count() == 0)
                return Result.Fail<RegistrationWorksGWFFDto>(RegistrationError.NoWorks);

            return Result.Ok(exports);
        }

        private async Task<RegistrationWorksGWFFDto> ConvertRegistrationsToEGEDA()
        {
            var registrations = await GetRegistrations();

            var result = new RegistrationWorksGWFFDto()
            {
                FileName = $"{RegistrationBatch.BatchId}/GWFF_{SanitizeAsFileName(_client.ClientName)}_{DateTime.Now:yyyMMdd}.xlsx",
                ClientName = _client.ClientName,
                Rows = registrations.AsEnumerable().Select(MapGWFF)
            };

            result.Rows = result.Rows.Where(w => w.ClientName != "Rejected").ToList();

            if (_rejectedWorksIds.Any())
            {
                await _mediator.Send(new DeleteRegistrationWorksCommand { BatchId = RegistrationBatch.BatchId, WorksIds = _rejectedWorksIds });
            }

            return result;
        }

        private GWFFRow MapGWFF(Core.Entities.Registration registration)
        {
            if ((registration.Works.Rights == null) | (registration.Works.Rights!.Count == 0))
            {
                registration.Works.Rights = InheritWorksRightsFromParent(registration.Works);
            }

            registration.Works.Rights = registration.Works.Rights.Where(r => (r.Type.Name is "BT" or "RR") && r.Countries.Any(c => c.Code is "DE" or "*")).ToList();

            if (!registration.Works.Rights.Any() || ExemptInGermany(registration.Works.Rights))
            {
                _rejectedWorksIds.Add(registration.Works.Id);
                return new GWFFRow() { ClientName = "Rejected" };
            }

            registration.Works.Rights = registration.Works.Rights.Where(r => r.Percentage is not (null or 0)).ToList();

            if (!registration.Works.Rights.Any())
            {
                _rejectedWorksIds.Add(registration.Works.Id);
                return new GWFFRow() { ClientName = "Rejected" };
            }

            var works = registration.Works;

            var row = _mapper.Map<GWFFRow>(works);

            if (works.Discriminator is "Season" or "Episode")
            {
                var seriesId = GetSeriesId(works);
                var series = _oscarContext.Series.Include(t => t.Titles).SingleOrDefault(s => s.Id == seriesId);
                var originalTitle = series.Titles!.FirstOrDefault(t => t.TitleType == TitleType.Main || t.TitleType == TitleType.Episode);
                row.SeriesCompactNo = series.CompactRef;
                row.TitleOfSeries = originalTitle.Title;
            }
            else if (works.Discriminator is "Series")
            {
                row.TitleOfSeries = row.OriginalTitle;
            }

            row.ClientName = _client.ClientName;

            return row;
        }

        private bool ExemptInGermany(ICollection<Right> rights) =>
            rights.Any(r => r.Percentage is (null or 0) && r.Countries.Any(c => c.Code is "DE"));

        private int? GetSeriesId(Core.Entities.Works works)
        {
            switch (works.Discriminator)
            {
                case "Episode":
                    return (works as Core.Entities.Episode).SeriesId;
                default:
                    return (works as Core.Entities.Season).SeriesId;
            }
        }

        protected async override Task<bool> IsClientRightsValid(Client client)
        {
            var rights = client.Rights.Where(r => r.Type.Name is "BT" or "RR").ToList();
            var isClientRightsValid = !ExemptInGermany(rights);

            return isClientRightsValid;
        }

    }
}
