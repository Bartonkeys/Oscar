using AutoMapper;
using BartonKeys.Functional;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Registration.Commands;
using Oscar.Infrastructure.Features.Rights.Queries;

namespace Oscar.Infrastructure.Features.Registration.Services
{
    public class SuissImageRegistrationService: RegistrationService<RegistrationWorksSuissImageExport>
    {
        private List<int> _rejectedWorksIds = new List<int>();

        public SuissImageRegistrationService(OscarContext oscarContext, IMapper mapper, ILogger<SuissImageRegistrationService> logger, IMediator mediator, IServiceScopeFactory serviceScopeFactory) 
            : base(oscarContext, mapper, logger, mediator, serviceScopeFactory)
        {
        }

        public override async Task<Result<RegistrationWorksSuissImageExport>> Create(RegistrationBatch registrationBatch, int clientId)
        {
            ClientId = clientId;
            RegistrationBatch = registrationBatch;

            var registrationsResult = await Register();

            if (registrationsResult.IsFailure)
                return Result.Fail<RegistrationWorksSuissImageExport>(registrationsResult.Error);

            var agicoaExports = await ConvertRegistrationsToSuissImageExports();

            if (!agicoaExports.Work.Where(w => w is not null)!.Any())
                return Result.Fail<RegistrationWorksSuissImageExport>(RegistrationError.NoWorks);

            return Result.Ok(agicoaExports);
        }

        protected override async Task<bool> IsClientRightsValid(Client client)
        {
            var rightsResult = await _mediator.Send(new GetRightsByClientIdQuery { ClientId = client.Id });
            return !rightsResult.IsFailure && CheckRights(rightsResult.Value);
        }

        private async Task<RegistrationWorksSuissImageExport> ConvertRegistrationsToSuissImageExports()
        {
            var registrations = await GetRegistrations();

            var work = registrations.Select(MapWorks).ToList();

            var suissImageWri = new RegistrationWorksSuissImageExport()
            {
                FileName = $"{RegistrationBatch.BatchId}/SUISSEIMAGE_{SanitizeAsFileName(_client.ClientName)}_{DateTime.Now:yyyMMdd}.xml",
                Header = new()
                {
                    Version = "WRI.02.02",
                    FromCompany = "Compact Collections Ltd",
                    FromPerson = "Registration Service",
                    ToCompany = "SuisseImage",
                    BegDate = DateTime.Now.ToString("yyyy/MM/dd"),
                    BegTime = DateTime.Now.ToShortTimeString(),
                    Extensions = "By using WRI, declarant recognizes having read and accepted the terms and conditions of the Mandates: http://www.agicoa.org/english/rightsholder/wri/WRI_mandate_v1_8_0.pdf, as selected for each work hereby declared."
                },
                Work = work,
                Footer = new()
                {
                    RecCount = registrations.Count().ToString(),
                    EndDate = DateTime.Now.ToString("yyyy/MM/dd"),
                    EndTime = DateTime.Now.ToShortTimeString(),
                }
            };

            suissImageWri.Work = suissImageWri.Work.Where(w => w.WorkDeclarationNumberSender != "Rejected").ToList();

            if (_rejectedWorksIds.Any())
            {
                await _mediator.Send(new DeleteRegistrationWorksCommand { BatchId = RegistrationBatch.BatchId, WorksIds = _rejectedWorksIds });
            }

            return suissImageWri;
        }

        private RegistrationWorksAgicoaExportDTO MapWorks(Core.Entities.Registration registration)
        {
            if ((registration.Works.Rights == null) | (registration.Works.Rights!.Count == 0))
            {
                registration.Works.Rights = InheritWorksRightsFromParent(registration.Works);
            }

            registration.Works.Rights = registration.Works.Rights.Where(r => (r.Type.Name is "BT" or "EC") && r.Countries.Any(c => c.Code is "CH" or "*")).ToList();

            if (!registration.Works.Rights.Any() || ExemptInCH(registration.Works.Rights))
            {
                _rejectedWorksIds.Add(registration.Works.Id);
                return new RegistrationWorksAgicoaExportDTO() { WorkDeclarationNumberSender = "Rejected" };
            }

            registration.Works.Rights = registration.Works.Rights.Where(r => r.Percentage is not (null or 0)).ToList();

            if (!registration.Works.Rights.Any())
            {
                _rejectedWorksIds.Add(registration.Works.Id);
                return new RegistrationWorksAgicoaExportDTO() { WorkDeclarationNumberSender = "Rejected" };
            }

            var works = registration.Works;

            var row = _mapper.Map<RegistrationWorksAgicoaExportDTO>(works);

            return row;
        }

        private static bool ExemptInCH(ICollection<Right> rights)
        {
            bool hasBTZeroRights = rights.Any(r => r.Type.Name is "BT" && r.Percentage is (null or 0) && r.Countries.Any(c => c.Code is "CH"));
            bool hasECZeroRights = rights.Any(r => r.Type.Name is "EC" && r.Percentage is (null or 0) && r.Countries.Any(c => c.Code is "CH"));

            var exempted = hasBTZeroRights && hasECZeroRights;
            return exempted;
        }

        private bool CheckRights(IEnumerable<RightDto> rightsResult)
        {
            var rights = rightsResult.Where(r => r.Type.Name is "BT" or "EC");
            return rights.Any() && rights.SelectMany(c => c.Countries).Any(c => c.Code is "CH" or "*");
        }

    }
}
