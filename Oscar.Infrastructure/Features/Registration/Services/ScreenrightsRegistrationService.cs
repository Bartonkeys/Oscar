using AutoMapper;
using BartonKeys.Functional;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Data.Context;
using Oscar.Data.Migrations;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Registration.Commands;
using static Oscar.Core.Common.Constants;

namespace Oscar.Infrastructure.Features.Registration.Services
{
    public class ScreenrightsRegistrationService: RegistrationService<RegistrationWorksScreenrightsExport>
    {
        private List<int> _rejectedWorksIds = new List<int>();

        public ScreenrightsRegistrationService(OscarContext oscarContext, IMapper mapper,
            ILogger<ScreenrightsRegistrationService> logger, IMediator mediator, IServiceScopeFactory serviceScopeFactory) : base(oscarContext, mapper, logger,
            mediator, serviceScopeFactory)
        {
        }

        public override async Task<Result<RegistrationWorksScreenrightsExport>> Create(RegistrationBatch registrationBatch, int clientId)
        {
            ClientId = clientId;
            RegistrationBatch = registrationBatch;

            var registrationsResult = await Register();

            if (registrationsResult.IsFailure)
                return Result.Fail<RegistrationWorksScreenrightsExport>(registrationsResult.Error);

            var screenrightsExports = await ConvertRegistrationsToScreenrights();

            if (!screenrightsExports.Work.Where(w => w is not null)!.Any())
                return Result.Fail<RegistrationWorksScreenrightsExport>(RegistrationError.NoWorks);

            return Result.Ok(screenrightsExports);
        }

        private async Task<RegistrationWorksScreenrightsExport> ConvertRegistrationsToScreenrights()
        {
            var registrations = await GetRegistrations();

            var work = registrations.Select(MapWorks).ToList();

            var screerightsWri = new RegistrationWorksScreenrightsExport()
            {
                FileName = $"{RegistrationBatch.BatchId}/SCREENRIGHTS_{SanitizeAsFileName(_client.ClientName)}_{DateTime.Now:yyyMMdd}.xml",
                Header = new()
                {
                    Version = "WRI.02.02",
                    FromCompany = "Compact Collections Ltd",
                    FromPerson = "Registration Service",
                    ToCompany = "Screenrights",
                    BegDate = DateTime.Now.ToString("yyyy/MM/dd"),
                    BegTime = DateTime.Now.ToShortTimeString(),
                    Extensions = "By using WRI, declarant recognizes having read and accepted the terms and conditions of the Mandates: http://www.agicoa.org/english/rightsholder/wri/WRI_mandate_v1_8_0.pdf, as selected for each work hereby declared."
                },
                Work = work,
                Footer = new()
                {
                    RecCount = work.Count(w => w.Type != "Rejected").ToString(),
                    EndDate = DateTime.Now.ToString("yyyy/MM/dd"),
                    EndTime = DateTime.Now.ToShortTimeString(),
                }
            };
            screerightsWri.Work = screerightsWri.Work.Where(w => w.Type != "Rejected").ToList();

            if (_rejectedWorksIds.Any())
            {
                await _mediator.Send(new DeleteRegistrationWorksCommand { BatchId = RegistrationBatch.BatchId, WorksIds = _rejectedWorksIds });
            }

            return screerightsWri;
        }

        /* Replace world rights of given percenatage with either NZ or AU rights
         1) Derive NZ rights of given % from WORLD rights when AU is available
         2) Derive AU rights of given % from WORLD rights when NZ is available
         3) Don't derive NZ or AU rights from WORLD rights when both AU and NZ are available instead assign their respective rights to Screenrights
         */
        private RegistrationWorksScreenrightsExportDTO MapWorks(Core.Entities.Registration registration)
        {
            if ((registration.Works.Rights == null) | (registration.Works.Rights!.Count == 0))
            {
                registration.Works.Rights = InheritWorksRightsFromParent(registration.Works);
            }

            if (ExemptCRAndECInAUNZ(registration.Works.Rights))
            {
                _rejectedWorksIds.Add(registration.Works.Id);
                return new RegistrationWorksScreenrightsExportDTO() { Type = "Rejected" };
            }

            var tempRights = new List<Right>();
            //Distinct() is used as duplicate entries were found for some works which seems to be user error and hence ignoring duplicates
            //Consider rights with 'All Languages' only
            if (!ExemptCRInAUNZ(registration.Works.Rights))
            {
                tempRights = registration.Works.Rights
                .Where(r => r.Type.Name == "CR" &&
                            r.Countries.Any(c => c.Code == "*" || c.Code == "AU" || c.Code == "NZ") &&
                            r.LanguageRights.Where(x => x.Language.Name == "*").Count() > 0)
                .Distinct().ToList();
            }
            else if (!ExemptECInAUNZ(registration.Works.Rights))
            {
                tempRights = registration.Works.Rights
                    .Where(r => r.Type.Name == "EC" &&
                                r.Countries.Any(c => c.Code == "*" || c.Code == "AU" || c.Code == "NZ") &&
                                r.LanguageRights.Where(x => x.Language.Name == "*").Count() > 0)
                    .Distinct().ToList();
            }

            //Do not filter further on tempRights for non-zeor rights as tempRights can have rights atleast in AU or NZ even if one of them has zero rights, 
            //Screenrights should accept zero rights if one of AU/NZ has zero rights but the other not
            //DO NOT FILTER AS --> registration.Works.Rights = tempRights.Where(r => r.Percentage is not (null or 0)).ToList();
            registration.Works.Rights = tempRights;

            if (!registration.Works.Rights.Any())
            {
                _rejectedWorksIds.Add(registration.Works.Id);
                return new RegistrationWorksScreenrightsExportDTO { Type = "Rejected" };
            }

            //When a respective right item has multiple countries added to it then split it for each country so that automapper can handle it accordingly for each country
            //Eg: 1 line item of right has say 10% rights for AU and NZ countries, then the rights needs to split with 10% rights for each country
            var splittedRightsForEachCountry = new List<Right>();
            foreach (var right in registration.Works.Rights)
            {
                if (right.Countries.Count() > 1)
                {
                    foreach (var country in right.Countries.Where(c => c.Code is "AU" or "NZ"))
                    {
                        var countryRight = new Right()
                        {
                            Client = new Client {IMaestroClientCode = right!.Client!.IMaestroClientCode, ClientReference = right!.Client!.ClientReference, ClientName = right!.Client!.ClientName },
                            Countries = new List<Core.Entities.Country> { new Core.Entities.Country { Code = country.Code } },
                            LanguageRights = right!.LanguageRights,
                            ChannelRights = right!.ChannelRights,
                            Percentage = right!.Percentage,
                            StartOfRight = right!.StartOfRight,
                            EndOfRight = right!.EndOfRight,
                            StartOfValidity = right!.StartOfValidity,
                            EndOfValidity = right!.EndOfValidity
                        };

                        countryRight.Countries.Clear();
                        countryRight.Countries.Add(country);
                        splittedRightsForEachCountry.Add(countryRight);
                    }
                }
                else
                {
                    splittedRightsForEachCountry.Add(right);
                }
            }
            registration.Works.Rights = splittedRightsForEachCountry;

            var work = _mapper.Map<RegistrationWorksScreenrightsExportDTO>(registration.Works);

            if (work.Rgts != null)
            {
                var worldRights = work.Rgts.Where(x => x.CountryOfRetransmission == "*").FirstOrDefault();
                var auRights = work.Rgts.Where(x => x.CountryOfRetransmission == "AU").FirstOrDefault();

                var nzRights = work.Rgts.Where(x => x.CountryOfRetransmission == "NZ").FirstOrDefault();
                if (nzRights != null)
                    AddNZEducationalCommunicationRights(work, nzRights);

                if (worldRights != null && auRights != null && nzRights != null)
                {
                    //when both AU rights and NZ rights are available then just remove World rights
                    work.Rgts.Remove(worldRights);
                }
                else if (worldRights != null && auRights != null && nzRights == null)
                {
                    //when AU rights is avaialble and NZ rights are not available then derive NZ rights from World rights and remove World rights
                    var nzRightsEduCopying = CloneHelper.Clone(worldRights);
                    nzRightsEduCopying.CountryOfRetransmission = "NZ";
                    nzRightsEduCopying.ServiceElection = (int)ServiceElectionEnum.NewZealandEducationalCopying;
                    work.Rgts.Add(nzRightsEduCopying);
                    AddNZEducationalCommunicationRights(work, nzRightsEduCopying);

                    work.Rgts.Remove(worldRights);
                }
                else if (worldRights != null && auRights == null && nzRights != null)
                {
                    //when AU rights is not avaialble and NZ rights are available then derive AU rights from World rights and remove World rights
                    var auRightsCopy = CloneHelper.Clone(worldRights);
                    auRightsCopy.CountryOfRetransmission = "AU";
                    auRightsCopy.ServiceElection = (int)ServiceElectionEnum.AllAustralianServices;
                    work.Rgts.Add(auRightsCopy);

                    work.Rgts.Remove(worldRights);
                }
                else if (worldRights != null && auRights == null && nzRights == null)
                {
                    //when neither AU rights is avaialble nor NZ rights are available then derive AU and NZ rights from World rights and remove World rights
                    var nzRightsCopy = CloneHelper.Clone(worldRights);
                    nzRightsCopy.CountryOfRetransmission = "NZ";
                    nzRightsCopy.ServiceElection = (int)ServiceElectionEnum.NewZealandEducationalCopying;
                    work.Rgts.Add(nzRightsCopy);

                    AddNZEducationalCommunicationRights(work, nzRightsCopy);

                    var auRightsCopy = CloneHelper.Clone(worldRights);
                    auRightsCopy.CountryOfRetransmission = "AU";
                    auRightsCopy.ServiceElection = (int)ServiceElectionEnum.AllAustralianServices;
                    work.Rgts.Add(auRightsCopy);

                    work.Rgts.Remove(worldRights);
                }
            }

            return work;
        }

        private bool ExemptCRAndECInAUNZ(ICollection<Right> rights)
        {
            bool auCRZeroRights = ExemptCRInAU(rights) == true;
            bool nzCRZeroRights = ExemptCRInNZ(rights) == true;
            bool auECZeroRights = ExemptECInAU(rights) == true;
            bool nzECZeroRights = ExemptECInNZ(rights) == true;

            var exempted = auCRZeroRights && nzCRZeroRights && auECZeroRights && nzECZeroRights; ;
            return exempted;
        }

        private bool ExemptCRInAUNZ(ICollection<Right> rights)
        {
            bool auCRZeroRights = ExemptCRInAU(rights) == true;
            bool nzCRZeroRights = ExemptCRInNZ(rights) == true;

            var exempted = auCRZeroRights && nzCRZeroRights; 
            return exempted;
        }

        private bool ExemptECInAUNZ(ICollection<Right> rights)
        {
            bool auECZeroRights = ExemptECInAU(rights) == true;
            bool nzECZeroRights = ExemptECInNZ(rights) == true;

            var exempted = auECZeroRights && nzECZeroRights; ;
            return exempted;
        }

        private static bool ExemptCRInAU(ICollection<Right> rights)
        {
            return !rights.Any() ||
                rights.Any(r => r.Type.Name is "CR" && r.Percentage is (null or 0) && r.Countries.Any(c => c.Code is "AU"));
        }

        private static bool ExemptCRInNZ(ICollection<Right> rights)
        {
            return !rights.Any() ||
               rights.Any(r => r.Type.Name is "CR" && r.Percentage is (null or 0) && r.Countries.Any(c => c.Code is "NZ"));
        }

        private static bool ExemptECInAU(ICollection<Right> rights)
        {
            return !rights.Any() ||
                rights.Any(r => r.Type.Name is "EC" && r.Percentage is (null or 0) && r.Countries.Any(c => c.Code is "AU"));
        }

        private static bool ExemptECInNZ(ICollection<Right> rights)
        {
            return !rights.Any() ||
                rights.Any(r => r.Type.Name is "EC" && r.Percentage is (null or 0) && r.Countries.Any(c => c.Code is "NZ"));
        }

        private static void AddNZEducationalCommunicationRights(RegistrationWorksScreenrightsExportDTO work, WorksRightScreenrightsExportDTO nzRights)
        {
            /* When NZ rights are added then we need to send 2 rights records:
             * One for NewZealandEducationalCopying which is set during mapping in RegistrationMappingProfile.cs
             * And 2nd for NewZealandEducationalCommunication which we need to add below*/
            var nzRightsCopy = CloneHelper.Clone(nzRights);
            nzRightsCopy.ServiceElection = (int)ServiceElectionEnum.NewZealandEducationalCommunication;
            work?.Rgts?.Add(nzRightsCopy);
        }

        protected override async Task<bool> IsClientRightsValid(Client client)
        {
            var isClientRightsValid = !ExemptCRAndECInAUNZ(client!.Rights);

            return isClientRightsValid;
        }

    }
}