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
using Oscar.Infrastructure.Features.Registration.Commands;

namespace Oscar.Infrastructure.Features.Registration.Services
{
    public class CMCRegistrationService : RegistrationService<RegistrationWorksCMCDto>
    {
        private List<int> _rejectedWorksIds = new List<int>();

        public CMCRegistrationService(OscarContext oscarContext, IMapper mapper, ILogger<RegistrationService<RegistrationWorksCMCDto>> logger, IMediator mediator, IServiceScopeFactory serviceScopeFactory) 
            : base(oscarContext, mapper, logger, mediator, serviceScopeFactory)
        {
        }

        public override async Task<Result<RegistrationWorksCMCDto>> Create(RegistrationBatch registrationBatch, int clientId)
        {
            ClientId = clientId;
            RegistrationBatch = registrationBatch;

            var registrationsResult = await Register();

            if (registrationsResult.IsFailure)
                return Result.Fail<RegistrationWorksCMCDto>(registrationsResult.Error);

            var screenrightsExports = await ConvertRegistrationsToCMC();

            return Result.Ok(screenrightsExports);
        }

        private async Task<RegistrationWorksCMCDto> ConvertRegistrationsToCMC()
        {
            await AddRegisteredWorksMissingClientReferences();
            var registrations = await GetRegistrations();

            var result = new RegistrationWorksCMCDto()
            {
                FileName = $"{RegistrationBatch.BatchId}/CMC_{_client.ClientReference}_{SanitizeAsFileName(_client.ClientName)}_{DateTime.Now:yyyMMdd}.xlsx",
                Rows = registrations.AsEnumerable().Select(MapCCC)
            };
            result.Rows = result.Rows.Where(w => w.RHID != "Rejected").ToList();

            if (_rejectedWorksIds.Any())
            {
                await _mediator.Send(new DeleteRegistrationWorksCommand { BatchId = RegistrationBatch.BatchId, WorksIds = _rejectedWorksIds });
            }

            return result;
        }

        private CMCRow MapCCC(Core.Entities.Registration registration)
        {
            if (registration.Works.Rights!.Count == 0)
            {
                registration.Works.Rights = InheritWorksRightsFromParent(registration.Works);
            }

            registration.Works.Rights = registration.Works.Rights.Where(r => r.Type.Name is "BT" && r.Percentage is not (null or 0)).ToList();

            if (!registration.Works.Rights.Any())
            {
                _rejectedWorksIds.Add(registration.Works.Id);
                return new CMCRow() { RHID = "Rejected" };
            }

            registration.Works.Rights = registration.Works.Rights.Where(r => r.Percentage is not (null or 0)).ToList();

            if (!registration.Works.Rights.Any())
            {
                _rejectedWorksIds.Add(registration.Works.Id);
                return new CMCRow() { RHID = "Rejected" };
            }

            var works = registration.Works;

            var row = _mapper.Map<CMCRow>(works);

            if (works.Discriminator is "Season" or "Episode")
            {
                var seriesId = GetSeriesId(works);
                var series = _oscarContext.Series.Include(t => t.Titles).SingleOrDefault(s => s.Id == seriesId);
                var originalTitle =
                    series.Titles!.FirstOrDefault(t => t.TitleType == TitleType.Main || t.TitleType == TitleType.Episode);
                var alternativeTitle =
                    series.Titles!.FirstOrDefault(t =>
                        t.TitleType == TitleType.MainAlternative || t.TitleType == TitleType.EpisodeAlternative);
                row.SerialOriginalTitle = originalTitle.Title;
                row.SerialOriginalTitleLanguage = originalTitle.LanguageCode;

                if (alternativeTitle != null)
                {
                    row.SerialAlternativeTitleLanguage = alternativeTitle.LanguageCode;
                    row.SerialAlternativeTitle = alternativeTitle.Title;
                }
            }

            row.Tags = $"{_client.Id},{_client.ClientName}";

            return row;
        }

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

        protected override async Task<bool> IsClientRightsValid(Client client)
        {
            client.Rights = client.Rights.Where(r => r.Type.Name is "BT").ToList();

            if (client.Rights.Any(worksRight => worksRight.Countries.Any(c => c.Name == "SPAIN") && worksRight.Percentage == 0))
                return false;

            if (client.Rights.Any(worksRight => worksRight.Countries.Any(c => c.Name == "WORLD") && worksRight.Percentage > 0))
                return true;

            if (client.Rights.Any(worksRight => worksRight.Countries.Any(c => c.Name == "SPAIN") && worksRight.Percentage > 0))
                return true;

            return false;
        }

        protected override bool IsSocietyRightsClaimableOnWork(Core.Entities.Works works, Core.Entities.Society society)
        {
            return true;
        }

        protected override bool IsValidWorkRightsForSocietyTerritory(Core.Entities.Works works, Core.Entities.Society society)
        {
            return works.Countries.Any(c => c.Code == "US") && IsValidWorkRights(works);
        }

        private bool IsValidWorkRights(Core.Entities.Works works)
        {
            if ((works.Rights == null) | (works.Rights!.Count == 0))
            {
                works.Rights = InheritWorksRightsFromParent(works);
            }

            works.Rights = works.Rights.Where(r => r.Type.Name is "BT").ToList();

            return !works.Rights.Any(worksRight =>
                       worksRight.Countries.Any(c => c.Name == "SPAIN") && worksRight.Percentage is (null or 0)) &&
                   (works.Rights.Any(worksRight =>
                       worksRight.Countries.Any(c => c.Name == "WORLD") && worksRight.Percentage > 0) ||
                   works.Rights.Any(
                       worksRight => worksRight.Countries.Any(c => c.Name == "SPAIN") && worksRight.Percentage > 0));
        }
    }
}
