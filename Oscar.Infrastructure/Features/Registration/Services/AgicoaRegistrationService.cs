using AutoMapper;
using BartonKeys.Functional;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Registration.Commands;

namespace Oscar.Infrastructure.Features.Registration.Services
{
    public class AgicoaRegistrationService : RegistrationService<RegistrationWorksAgicoaExport>
    {
        private List<Core.Entities.Registration> _registrations;
        private List<RejectedWork> _rejectedWorks = new List<RejectedWork>();

        public AgicoaRegistrationService(OscarContext oscarContext, IMapper mapper,
            ILogger<AgicoaRegistrationService> logger, IMediator mediator, IServiceScopeFactory serviceScopeFactory) : base(oscarContext, mapper, logger,
            mediator, serviceScopeFactory)
        {
        }

        public override async Task<Result<RegistrationWorksAgicoaExport>> Create(RegistrationBatch registrationBatch, int clientId)
        {
            ClientId = clientId;
            RegistrationBatch = registrationBatch;

            var registrationsResult = await Register();

            if (registrationsResult.IsFailure)
                return Result.Fail<RegistrationWorksAgicoaExport>(registrationsResult.Error);

            var agicoaExports = await ConvertRegistrationsToAgicoaExports();

            return Result.Ok(agicoaExports);
        }

        protected override async Task<bool> IsClientRightsValid(Client client)
        {
            return true;
        }

        private async Task<RegistrationWorksAgicoaExport> ConvertRegistrationsToAgicoaExports()
        {
            _registrations = await GetRegistrations();

            var agicoaWri = new RegistrationWorksAgicoaExport()
            {
                FileName = $"{RegistrationBatch.BatchId}/{_client.AgicoaClientRef}_WRI0202_{DateTime.Now:yyyMMdd}.xml",
                Header = new()
                {
                    Version = "WRI.02.02",
                    FromCompany = "Compact Collections Ltd",
                    FromPerson = "Registration Service", //_userProvider.GetName() ?? "Unknown",
                    ToCompany = "Agicoa",
                    BegDate = DateTime.Now.ToString("yyyy/MM/dd"),
                    BegTime = DateTime.Now.ToShortTimeString(),
                    Extensions = "By using WRI, declarant recognizes having read and accepted the terms and conditions of the Mandates: http://www.agicoa.org/english/rightsholder/wri/WRI_mandate_v1_8_0.pdf, as selected for each work hereby declared."
                },
                Work = _registrations.AsEnumerable().Select(MapAgicoa).ToList(),
                Footer = new()
                {
                    EndDate = DateTime.Now.ToString("yyyy/MM/dd"),
                    EndTime = DateTime.Now.ToShortTimeString(),
                }
            };

            if (_rejectedWorks.Any())
            {
                NotToRejectParentSeasonsAndSeriesWhenEpisodeIsRegistrable(agicoaWri);
                NotToRejectParentSeriesWhenSeasonIsRegistrable(agicoaWri);

                //SerialNoSender stores CompactRef
                //Exclude rejected works
                agicoaWri.Work = agicoaWri.Work.Where(w => !_rejectedWorks.Select(x => x.CompactRef).Contains(w.WorkDeclarationNumberSender)).ToList();
                agicoaWri.Footer.RecCount = agicoaWri.Work.Count().ToString();

                await _mediator.Send(new DeleteRegistrationWorksCommand { BatchId = RegistrationBatch.BatchId, WorksIds = _rejectedWorks.Select(x => x.Id).ToList() });
            }

            return agicoaWri;
        }

        //Registration requirement: Don't reject parent season & series when episode is selected for registration even when season & series don't have rights
        private void NotToRejectParentSeasonsAndSeriesWhenEpisodeIsRegistrable(RegistrationWorksAgicoaExport agicoaWri)
        {
            //SerialLevel == 3 ==> Episodes
            var selectedEpisodesForRegistration = agicoaWri.Work.Where(x => x.SerialLevel == 3 && !x.IsRejected).ToList();

            var parentSeasonIdsNotToBeRejected = _rejectedWorks.Where(x => x.Discriminator == "Season" && selectedEpisodesForRegistration.Select(x => x.SeasonNoSender).Contains("CC-" + x.CompactRef)).Select(x => x.Id);

            //Clone as otherwise it will raise exception as we end up modifying the same collection i.e _rejectedWorks
            var parentSeasonIdsNotToBeRejectedCopy = CloneHelper.Clone(parentSeasonIdsNotToBeRejected);
            foreach (var seasonId in parentSeasonIdsNotToBeRejectedCopy)
            {
                var rejectedSeason = _rejectedWorks.FirstOrDefault(x => x.Id == seasonId);
                if (rejectedSeason != null)
                {
                    _rejectedWorks.Remove(rejectedSeason);
                }
            }

            var parentSeriesIdsNotToBeRejected = _rejectedWorks.Where(x => x.Discriminator == "Series" && selectedEpisodesForRegistration.Select(x => x.SerialNoSender).Contains("CC-" + x.CompactRef)).Select(x => x.Id);

            //Clone as otherwise it will raise exception as we end up modifying the same collection i.e _rejectedWorks
            var parentSeriesIdsNotToBeRejectedCopy = CloneHelper.Clone(parentSeriesIdsNotToBeRejected);
            foreach (var seriesId in parentSeriesIdsNotToBeRejectedCopy)
            {
                var rejectedSeries = _rejectedWorks.FirstOrDefault(x => x.Id == seriesId);
                if (rejectedSeries != null)
                {
                    _rejectedWorks.Remove(rejectedSeries);
                }
            }
        }

        //Registration requirement: Don't reject parent series when season is selected for registration even when series don't have rights
        private void NotToRejectParentSeriesWhenSeasonIsRegistrable(RegistrationWorksAgicoaExport agicoaWri)
        {
            //SerialLevel == 2 ==> Season
            var selectedSeasonsForRegistration = agicoaWri.Work.Where(x => x.SerialLevel == 2 && !x.IsRejected).ToList();

            var parentSeriesIdsNotToBeRejected = _rejectedWorks.Where(x => x.Discriminator == "Series" && selectedSeasonsForRegistration.Select(x => x.SerialNoSender).Contains("CC-" + x.CompactRef)).Select(x => x.Id);

            //Clone as otherwise it will raise exception as we end up modifying the same collection i.e _rejectedWorks
            var parentSeriesIdsNotToBeRejectedCopy = CloneHelper.Clone(parentSeriesIdsNotToBeRejected);
            foreach (var seriesId in parentSeriesIdsNotToBeRejectedCopy)
            {
                var rejectedSeries = _rejectedWorks.FirstOrDefault(x => x.Id == seriesId);
                if (rejectedSeries != null)
                {
                    _rejectedWorks.Remove(rejectedSeries);
                }
            }
        }

        /*1	Exclude rights for respective work from wri if inherited for any work type
          2	Include rights in wri if overriden inherted rights by user for any work type
          3	Always include reference of Parent of season/epiosde
          4	Always include work in wri that has valid rights irrespective of the rights came from inheritance or overriden by user
         */
        private RegistrationWorksAgicoaExportDTO MapAgicoa(Core.Entities.Registration registration)
        {
            bool isRejected = false;
            if (!registration.Works.Directors.Any())
            {
                _rejectedWorks.Add(new RejectedWork(registration.Works.Id, registration.Works.CompactRef, registration.Works.Discriminator));
                isRejected = true;
            }

            bool isInheritedRightsFromParent = false;
            if ((registration.Works.Rights == null) | (registration.Works.Rights!.Count == 0))
            {
                //Inherit parent rights so that we can use parent rights to validate the current work but later remove the inherited rights from current work
                var parentRights = InheritWorksRightsFromParent(registration.Works);

                //Inherit rights into work out their validity but then remove the rights from Agicoa WRI after using the rights for validation
                //as Agicoa don't wan't inherited rights to be included for any of works unless the inherited rights are overriden by their own rights by the user
                registration.Works.Rights = parentRights;
                isInheritedRightsFromParent = true;
            }

            var retransimissionRights = registration.Works!.Rights!.Where(r => (r.Type.Name is "CR") && r.Percentage is not (null or 0));

            registration.Works.Rights = new List<Right>();
            foreach (var retransimissionRight in retransimissionRights)
            {
                foreach (var country in retransimissionRight.Countries)
                {
                    var agicoaCARight = _mapper.Map<Right>(retransimissionRight);
                    agicoaCARight.Type.Name = "CA";
                    agicoaCARight.Countries = new List<Core.Entities.Country> { country };
                    registration.Works.Rights.Add(agicoaCARight);
                }
            }

            if (!registration.Works.Rights.Any())
            {
                _rejectedWorks.Add(new RejectedWork(registration.Works.Id, registration.Works.CompactRef, registration.Works.Discriminator));
                isRejected = true;
            }

            RemoveRightsIfInherited(registration.Works, isInheritedRightsFromParent);

            registration.Works.ClientReferences =
                registration.Works.ClientReferences!.Where(c => c.Client.Id == _client.Id).ToList();

            var result = _mapper.Map<RegistrationWorksAgicoaExportDTO>(registration.Works);
            result.IsRejected = isRejected;

            return result;
        }

        protected override bool IsValidWorkRightsForSocietyTerritory(Core.Entities.Works works, Core.Entities.Society society)
        {
            bool isInheritedRightsFromParent = false;

            //Add inherited rights if current work has no rights so that we can validate current work using its parent rights
            if ((works.Rights == null) | (works.Rights!.Count == 0))
            {
                works.Rights = InheritWorksRightsFromParent(works);
                isInheritedRightsFromParent = true;
            }

            var isValid = CheckSocietyRightsAndTerritory(society, works.Rights);

            //Remove inherited rights once the current work is done validating
            RemoveRightsIfInherited(works, isInheritedRightsFromParent);

            return isValid;
        }

        private static void RemoveRightsIfInherited(Core.Entities.Works works, bool inheritedRightsFromParent)
        {
            //Child season work should not include rights in Agicoa WRI if the rights were already included in it's parent series
            //Child episode work should not include rights in Agicoa WRI if the rights were already included in it's parent season/series
            if (inheritedRightsFromParent)
            {
                works.Rights = new List<Right>();
            }
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

    }

    public class RejectedWork
    {
        public int Id { get; }
        public string CompactRef { get; }
        public string Discriminator { get; }

        public RejectedWork(int id, string compactRef, string discriminator)
        {
            Id = id;
            CompactRef = compactRef;
            Discriminator = discriminator;
        }
    }
}

