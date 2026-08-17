using AutoMapper;
using BartonKeys.Functional;
using Microsoft.EntityFrameworkCore;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;

namespace Oscar.Infrastructure.Features.Common
{
    public partial class RegistrationMappingProfile : Profile
    {
        public RegistrationMappingProfile()
        {
            CreateMap<RegistrationWorksScreenrightsExportDTO, Oscar.Core.Entities.Works>().ReverseMap()
              .ForMember(d => d.WorkDeclarationNumberSender, o => o.MapFrom(s => s != null ? "CC-" + s.CompactRef : null))
              
              //Removing below as it is optional and has incorrect value set
              //.ForMember(d => d.WorkDeclarationNumberReceiver, o => o.MapFrom(s => s.ClientReferences != null && s.ClientReferences!.FirstOrDefault() != null && !string.IsNullOrEmpty(s.ClientReferences!.FirstOrDefault().AgicoaDeclarationNumber) ? s.ClientReferences.FirstOrDefault()!.AgicoaDeclarationNumber : null))
              
              .ForMember(d => d.Ttls, o => o.MapFrom(s => s.Titles))
              .ForMember(d => d.Olngs, o => o.MapFrom(s => s.Languages.Select(l => l.Name.ToUpper())))
              .ForMember(d => d.CntyPrds, o => o.MapFrom(s => s.Countries.Select(c => c.Code.ToUpper())))
              .ForMember(d => d.CmpyPrds, o => o.MapFrom(s => s.Companies))
              .ForMember(d => d.Rgts, o => o.MapFrom(s => s.Rights))
              .ForMember(d => d.Mandates, o => o.MapFrom(s => s.Mandates))
              .ForMember(d => d.IParts, o => o.MapFrom(s => ConvertInterestedScreenrightsParties(s.Actors, s.Directors, s.Producers, s.Distributors, s.ScriptWriters, s.ScreenWriters)))
              .ForMember(d => d.SerialLevel, o => o.MapFrom(w => ConvertDiscriminator(w.Discriminator)))

              .ForMember(d => d.SerialNoSender, o => o.MapFrom<SerialNumberSenderScreenrightsConvertor>()) //SNS

              //Removing below as it is optional and has incorrect value set
              //.ForMember(d => d.SerialNoReceiver, o => o.MapFrom<SerialNumberReceiverScreenrightsConvertor>()) // SNR

              .ForMember(d => d.SeasonNoSender, o => o.MapFrom<SeasonNumberSenderScreenrightsConvertor>()) //SNNS

              //Removing below as it is optional and has incorrect value set
              //.ForMember(d => d.SeasonNoReceiver, o => o.MapFrom<SeasonNumberReceiverScreenrightsConvertor>()) //SNNR

              .ForMember(d => d.SeasonNo, o => o.MapFrom<SeasonNumberScreenrightsConvertor>()) //SNN
              .ForMember(d => d.EpisodeNo, o => o.MapFrom(w => w.Discriminator == "Episode" ? (w.Number > 0 ? w.Number : null) : null))
              .ForMember(d => d.Duration, o => o.MapFrom(w => w.DurationMinutes))
              .ForMember(d => d.Type, o => o.MapFrom(w => w.WorksType!.Name))
              .ForMember(d => d.Kind, o => o.MapFrom(w => w.Genre != null ? w.Genre.Name : null))
              .ForMember(d => d.YearOfProduction, o => o.MapFrom(w => w.ProductionYear))
              .ForMember(d => d.Withdrawal, o => o.MapFrom(s => s.WorksStatus == Core.Enums.WorksStatus.Relinquished ? "yes" : "no"));

            CreateMap<WorksTitleScreenrightsExportDTO, WorksTitle>().ReverseMap()
              .ForMember(d => d.Original, o => o.MapFrom(s => s.TitleType == Core.Enums.TitleType.Main || s.TitleType == Core.Enums.TitleType.Episode ? "Yes" : "No"))
              .ForMember(d => d.LanguageCode, o => o.MapFrom(s => s.LanguageCode.ToUpper()))
              .ForMember(d => d.Title, o => o.MapFrom(s => s.Title));

            CreateMap<WorksCompanyOfProductionScreenrightsExportDTO, Company>().ReverseMap()
              .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Name));

            CreateMap<WorksRightScreenrightsExportDTO, Right>().ReverseMap()
             .ForMember(d => d.RightsholderReferenceSender, o => o.MapFrom<RightsholderReferenceSenderScreenrightsConvertor>())
             .ForMember(d => d.RightsholderNameSender, o => o.MapFrom(s => s.Client.ClientName.ToUpper() ?? string.Empty))
             .ForMember(d => d.CountryOfRetransmission, o => o.MapFrom(s => s.Countries.Any() ? s.Countries.First().Code.ToUpper() : string.Empty))
             .ForMember(d => d.LanguageCode, o => o.MapFrom(s => s.LanguageRights.Any() ? s.LanguageRights.First().Language.Name.ToUpper() : string.Empty))
             .ForMember(d => d.TVChannel, o => o.MapFrom(s => s.ChannelRights.Any() ? s.ChannelRights.First().Channel.Name : string.Empty))
             .ForMember(d => d.PercentageOfFilmRights, o => o.MapFrom(s => s.Percentage))
             .ForMember(d => d.PercentageOfScriptRights, o => o.MapFrom(s => s.Percentage))
             .ForMember(d => d.PercentageOfCommissionedSoundRights, o => o.MapFrom(s => s.Percentage))
             .ForMember(d => d.RightsFrom, o => o.MapFrom(s => s.StartOfRight.ToString("yyyy/MM/dd")))
             .ForMember(d => d.RightsTo, o => o.MapFrom(s => s.EndOfRight.ToString("yyyy/MM/dd")))
             .ForMember(d => d.ValidityFrom, o => o.MapFrom(s => s.StartOfValidity.ToString("yyyy/MM/dd")))
             .ForMember(d => d.ValidityTo, o => o.MapFrom(s => s.EndOfValidity.ToString("yyyy/MM/dd")))
             .ForMember(d => d.ServiceElection, o => o.MapFrom<ServiceElectionScreenrightsConvertor>());

            CreateMap<WorksMandateScreenrightsExportDTO, Mandate>().ReverseMap()
                .ForMember(d => d.MandateType, o => o.MapFrom(s => s.MandateType.Name))
                .ForMember(d => d.Mandated, o => o.MapFrom(s => s.Mandated ? "Y" : "N"));

            CreateMap<RegistrationWorksAgicoaExportDTO, Oscar.Core.Entities.Works>().ReverseMap()
              .ForMember(d => d.WorkDeclarationNumberSender, o => o.MapFrom(s => s != null ? "CC-" + s.CompactRef : null))
              .ForMember(d => d.WorkDeclarationNumberReceiver, o => o.MapFrom(s => s.ClientReferences != null && s.ClientReferences.FirstOrDefault() != null ? s.ClientReferences.FirstOrDefault()!.AgicoaDeclarationNumber : null))
              .ForMember(d => d.Ttls, o => o.MapFrom(s => s.Titles))
              .ForMember(d => d.Olngs, o => o.MapFrom(s => s.Languages.Select(l => l.Name)))
              .ForMember(d => d.CntyPrds, o => o.MapFrom(s => s.Countries.Select(c => c.Code)))
              .ForMember(d => d.CmpyPrds, o => o.MapFrom(s => s.Companies))
              .ForMember(d => d.Rgts, o => o.MapFrom(s => s.Rights))
              .ForMember(d => d.Mandates, o => o.MapFrom(s => s.Mandates))
              .ForMember(d => d.IParts, o => o.MapFrom(s => ConvertInterestedAgicoaParties(s.Actors, s.Directors, s.Producers, s.Distributors, s.ScriptWriters, s.ScreenWriters)))
              .ForMember(d => d.SerialLevel, o => o.MapFrom(w => ConvertDiscriminator(w.Discriminator)))

              .ForMember(d => d.SerialNoSender, o => o.MapFrom<SerialNumberSenderAgicoaConvertor>()) //SNS
              .ForMember(d => d.SerialNoReceiver, o => o.MapFrom<SerialNumberReceiverAgicoaConvertor>()) // SNR

              .ForMember(d => d.SeasonNoSender, o => o.MapFrom<SeasonNumberSenderAgicoaConvertor>()) //SNNS
              .ForMember(d => d.SeasonNoReceiver, o => o.MapFrom<SeasonNumberReceiverAgicoaConvertor>()) //SNNR

              .ForMember(d => d.SeasonNo, o => o.MapFrom(w => w.Discriminator == "Season" ? (w.Number > 0 ? w.Number : null) : null))
              .ForMember(d => d.EpisodeNo, o => o.MapFrom(w => w.Discriminator == "Episode" ? (w.Number > 0 ? w.Number : null) : null))
              .ForMember(d => d.Duration, o => o.MapFrom(w => w.DurationMinutes))
              .ForMember(d => d.Type, o => o.MapFrom(w => w.WorksType!.Name))
              .ForMember(d => d.Kind, o => o.MapFrom(w => w.Genre != null ? w.Genre.Name : null))
              .ForMember(d => d.YearOfProduction, o => o.MapFrom(w => w.ProductionYear))
              .ForMember(d => d.Withdrawal, o => o.MapFrom(s => s.WorksStatus == Core.Enums.WorksStatus.Relinquished? "yes" : "no"));

            CreateMap<WorksTitleAgicoaExportDTO, WorksTitle>().ReverseMap()
              .ForMember(d => d.Original, o => o.MapFrom(s => s.TitleType == Core.Enums.TitleType.Main || s.TitleType == Core.Enums.TitleType.Episode ? "Y" : "N"))
              .ForMember(d => d.LanguageCode, o => o.MapFrom(s => s.LanguageCode))
              .ForMember(d => d.Title, o => o.MapFrom(s => s.Title));

            CreateMap<WorksCompanyOfProductionAgicoaExportDTO, Company>().ReverseMap()
              .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Name));

            CreateMap<WorksRightAgicoaExportDTO, Right>().ReverseMap()
             .ForMember(d => d.RightsholderNameSender, o => o.MapFrom(s => s.Client.ClientName.ToUpper() ?? string.Empty))
             .ForMember(d => d.CountryOfRetransmission, o => o.MapFrom(s => s.Countries.Any() ? s.Countries.First().Code : string.Empty))
             .ForMember(d => d.LanguageCode, o => o.MapFrom(s => s.LanguageRights.Any() ? s.LanguageRights.First().Language.Name : string.Empty))
             .ForMember(d => d.TVChannel, o => o.MapFrom(s => s.ChannelRights.Any() ? s.ChannelRights.First().Channel.Name : string.Empty))
             .ForMember(d => d.PercentageOfRight, o => o.MapFrom(s => s.Percentage))
             .ForMember(d => d.RightsFrom, o => o.MapFrom(s => s.StartOfRight))
             .ForMember(d => d.RightsTo, o => o.MapFrom(s => s.EndOfRight))
             .ForMember(d => d.ValidityFrom, o => o.MapFrom(s => s.StartOfValidity))
             .ForMember(d => d.ValidityTo, o => o.MapFrom(s => s.EndOfValidity))
             .ForMember(d => d.Rights, o => o.MapFrom(s => s.Type.Name ));

            CreateMap<WorksMandateAgicoaExportDTO, Mandate>().ReverseMap()
                .ForMember(d => d.MandateType, o => o.MapFrom(s => s.MandateType.Name))
                .ForMember(d => d.Mandated, o => o.MapFrom(s => s.Mandated ? "Y":"N"));

            CreateMap<Core.Entities.Registration, CCCRow>()
                .ForMember(d => d.ClaimantInternalReferenceNumber, o => o.MapFrom(s => s.Works!.CompactRef))
                .ForMember(d => d.OwnershipPercentage, o => o.MapFrom(s => s.Works!.Rights!.First().Percentage.ToString()))
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Works!.Titles!.First(t => t.TitleType == TitleType.Main || t.TitleType == TitleType.Episode).Title))
                .ForMember(d => d.Genre, o => o.MapFrom(s => s.Works!.Genre!.Description))
                .ForMember(d => d.CopyrightYear, o => o.MapFrom(s => s.Works!.ProductionYear))
                .ForMember(d => d.Country, o => o.MapFrom(s => string.Join(", ", s.Works!.Countries!.Select(c => c.Description))))
                .ForMember(d => d.Duration, o => o.MapFrom(s => s.Works!.DurationMinutes))
                .ForMember(d => d.StartDate, o => o.MapFrom(s => s.Works!.Rights!.First().StartOfRight.ToString("yyyy/MM/dd")))
                .ForMember(d => d.EndDate, o => o.MapFrom(s => s.Works!.Rights!.First().EndOfRight.ToString("yyyy/MM/dd")))
                .ForMember(d => d.Broadcast, o => o.MapFrom(s => "A"))
                .ForMember(d => d.PrincipalCast, o => o.MapFrom<PrincipalCastConvertor>());

            CreateMap<Core.Entities.Works, CMCRow>()
                .ForMember(d => d.RHID, o => o.MapFrom(s => s.ClientReferences != null && s.ClientReferences.Count > 0 ? s.ClientReferences.First().Id.ToString() : null))
                .ForMember(d => d.Type, o => o.MapFrom(s => s.WorksType.Name))
                .ForMember(d => d.Genre, o => o.MapFrom(s => s.Genre.Name))
                .ForMember(d => d.Duration, o => o.MapFrom(s => s.DurationMinutes))
                .ForMember(d => d.ISAN, o => o.MapFrom(s => s.Isan))
                .ForMember(d => d.YearOfProduction, o => o.MapFrom(s => s.ProductionYear))
                .ForMember(d => d.OriginalTitle, o => o.MapFrom(s => s.Titles!.First(t => t.TitleType == TitleType.Main || t.TitleType == TitleType.Episode).Title))
                .ForMember(d => d.OriginalTitleLanguage, o => o.MapFrom(s => s.Titles!.First(t => t.TitleType == TitleType.Main || t.TitleType == TitleType.Episode).LanguageCode))
                .ForMember(d => d.AlternativeTitleLanguage, o => o.MapFrom(s => s.Titles!.Any(t => t.TitleType == TitleType.MainAlternative || t.TitleType == TitleType.EpisodeAlternative)
                ? s.Titles!.First(t => t.TitleType == TitleType.MainAlternative || t.TitleType == TitleType.EpisodeAlternative).LanguageCode : null))
                .ForMember(d => d.AlternativeTitle, o => o.MapFrom(s => s.Titles!.Any(t => t.TitleType == TitleType.MainAlternative || t.TitleType == TitleType.EpisodeAlternative)
                    ? s.Titles!.First(t => t.TitleType == TitleType.MainAlternative || t.TitleType == TitleType.EpisodeAlternative).Title : null))
                .ForMember(d => d.SerialLevel, o => o.MapFrom(w => ConvertDiscriminator(w.Discriminator)))
                .ForMember(d => d.SeasonNumber, o => o.MapFrom(w => w.Discriminator == "Season" ? (w.Number > 0 ? w.Number : 0) : 0))
                .ForMember(d => d.EpisodeNumber, o => o.MapFrom(w => w.Discriminator == "Episode" ? (w.Number > 0 ? w.Number : 0) : 0))
                .ForMember(d => d.Director1FirstName, o => o.MapFrom(s => s.Directors.Any() ? s.Directors.First().FirstName : null))
                .ForMember(d => d.Director1LastName, o => o.MapFrom(s => s.Directors.Any() ? s.Directors.First().LastName : null))
                .ForMember(d => d.Director2FirstName, o => o.MapFrom(s => s.Directors.Count > 1 ? s.Directors.Skip(1).Take(1).Single().FirstName : null))
                .ForMember(d => d.Director2LastName, o => o.MapFrom(s => s.Directors.Count > 1 ? s.Directors.Skip(1).Take(1).Single().LastName : null))
                .ForMember(d => d.Writer1FirstName, o => o.MapFrom(s => s.ScreenWriters.Any() ? s.ScreenWriters.First().FirstName : null))
                .ForMember(d => d.Writer1LastName, o => o.MapFrom(s => s.ScreenWriters.Any() ? s.ScreenWriters.First().LastName : null))
                .ForMember(d => d.Writer2FirstName, o => o.MapFrom(s => s.ScreenWriters.Count > 1 ? s.ScreenWriters.Skip(1).Take(1).Single().FirstName : null))
                .ForMember(d => d.Writer2LastName, o => o.MapFrom(s => s.ScreenWriters.Count > 1 ? s.ScreenWriters.Skip(1).Take(1).Single().LastName : null))
                .ForMember(d => d.Actor1FirstName, o => o.MapFrom(s => s.Actors.Any() ? s.Actors.First().FirstName : null))
                .ForMember(d => d.Actor1LastName, o => o.MapFrom(s => s.Actors.Any() ? s.Actors.First().LastName : null))
                .ForMember(d => d.Actor2FirstName, o => o.MapFrom(s => s.Actors.Count > 1 ? s.Actors.Skip(1).Take(1).Single().FirstName : null))
                .ForMember(d => d.Actor2LastName, o => o.MapFrom(s => s.Actors.Count > 1 ? s.Actors.Skip(1).Take(1).Single().LastName : null))
                .ForMember(d => d.Actor3FirstName, o => o.MapFrom(s => s.Actors.Count > 2 ? s.Actors.Skip(2).Take(1).Single().FirstName : null))
                .ForMember(d => d.Actor3LastName, o => o.MapFrom(s => s.Actors.Count > 2 ? s.Actors.Skip(2).Take(1).Single().LastName : null))
                .ForMember(d => d.ProductionCountry1, o => o.MapFrom(s => s.Countries.Any() ? s.Countries.First().Code3A : null))
                .ForMember(d => d.ProductionCountry2, o => o.MapFrom(s => s.Countries.Count > 1 ? s.Countries.Skip(1).Take(1).Single().Code3A : null))
                .ForMember(d => d.ProductionCountry3, o => o.MapFrom(s => s.Countries.Count > 2 ? s.Countries.Skip(2).Take(1).Single().Code3A : null))
                .ForMember(d => d.OriginalLanguage, o => o.MapFrom(s => s.Languages.Any() ? s.Languages.First().Name : null))
                .ForMember(d => d.ProductionCompany1, o => o.MapFrom(s => s.Companies.Any() ? s.Companies.First().Name : null))
                .ForMember(d => d.ProductionCompany2, o => o.MapFrom(s => s.Companies.Count > 1 ? s.Companies.Skip(1).Take(1).Single().Name : null))
                .ForMember(d => d.ProductionCompany3, o => o.MapFrom(s => s.Companies.Count > 2 ? s.Companies.Skip(2).Take(1).Single().Name : null))
                .ForMember(d => d.RightsStartDate, o => o.MapFrom(s => s.Rights.First().StartOfRight.ToString("dd/MM/yyyy")))
                .ForMember(d => d.RightsEndDate, o => o.MapFrom(s => s.Rights.First().EndOfRight.ToString("dd/MM/yyyy") == "31/12/9999" ? null : s.Rights.First().EndOfRight.ToString("dd/MM/yyyy")));

            CreateMap<Core.Entities.Works, MPLCRow>()
                .ForMember(d => d.CompactRef, o => o.MapFrom(s => s.CompactRef))
                .ForMember(d => d.WorkType, o => o.MapFrom(s => s.Discriminator))
                .ForMember(d => d.YearOfProduction, o => o.MapFrom(s => s.ProductionYear))
                .ForMember(d => d.OwningClient, o => o.MapFrom(s => s.Clients.FirstOrDefault().ClientName.ToUpper() ?? string.Empty))
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Titles!.First(t => t.TitleType == TitleType.Main || t.TitleType == TitleType.Episode).Title))
                .ForMember(d => d.Director1FirstName, o => o.MapFrom(s => s.Directors.Any() ? s.Directors.First().FirstName : null))
                .ForMember(d => d.Director1LastName, o => o.MapFrom(s => s.Directors.Any() ? s.Directors.First().LastName : null))
                .ForMember(d => d.Director2FirstName, o => o.MapFrom(s => s.Directors.Count > 1 ? s.Directors.Skip(1).Take(1).Single().FirstName : null))
                .ForMember(d => d.Director2LastName, o => o.MapFrom(s => s.Directors.Count > 1 ? s.Directors.Skip(1).Take(1).Single().LastName : null))
                .ForMember(d => d.Director2FirstName, o => o.MapFrom(s => s.Directors.Count > 2 ? s.Directors.Skip(2).Take(1).Single().FirstName : null))
                .ForMember(d => d.Director2LastName, o => o.MapFrom(s => s.Directors.Count > 2 ? s.Directors.Skip(2).Take(1).Single().LastName : null))
                .ForMember(d => d.ProductionCountry1, o => o.MapFrom(s => s.Countries.Any() ? s.Countries.First().Code3A : null))
                .ForMember(d => d.ProductionCountry2, o => o.MapFrom(s => s.Countries.Count > 1 ? s.Countries.Skip(1).Take(1).Single().Code3A : null))
                .ForMember(d => d.ProductionCountry3, o => o.MapFrom(s => s.Countries.Count > 2 ? s.Countries.Skip(2).Take(1).Single().Code3A : null));

            CreateMap<Core.Entities.Works, CRCRow>()
                .ForMember(d => d.CompactRef, o => o.MapFrom(s => s.CompactRef))
                .ForMember(d => d.FirstStartDate, o => o.MapFrom(s => s.Rights.First().StartOfRight.ToString("dd/MM/yyyy")))
                .ForMember(d => d.EndDate, o => o.MapFrom(s => s.Rights.First().EndOfRight.ToString("dd/MM/yyyy")))
                .ForMember(d => d.OriginalTitle, o => o.MapFrom<SeasonTitleConvertor>())
                .ForMember(d => d.EpisodeTitle, o => o.MapFrom<EpisodeTitleConvertor>())
                .ForMember(d => d.SeasonCount, o => o.MapFrom(w => w.Discriminator == "Season" ? (w.Number > 0 ? w.Number : null) : null))
                .ForMember(d => d.EpisodeCount, o => o.MapFrom(w => w.Discriminator == "Episode" ? (w.Number > 0 ? w.Number : null) : null))
                .ForMember(d => d.AltTitles, o => o.MapFrom(s => string.Join(", ", 
                    s.Titles!.Where(t => t.TitleType == TitleType.MainAlternative || t.TitleType == TitleType.EpisodeAlternative).Select(t => t.Title))))
                .ForMember(d => d.TitleType, o => o.MapFrom(w => ConvertDiscriminatorToTitleType(w.Discriminator)))
                .ForMember(d => d.WorkType, o => o.MapFrom(s => s.WorksType.Name))
                .ForMember(d => d.ProductionCompanies, o => o.MapFrom(s => string.Join(", ", s.Companies!.Select(t => t.Name))))
                .ForMember(d => d.Directors, o => o.MapFrom(s => string.Join(", ", s.Directors!.Select(t => $"{t.FirstName} {t.LastName}"))))
                .ForMember(d => d.Actors, o => o.MapFrom(s => string.Join(", ", s.Actors!.Select(t => $"{t.FirstName} {t.LastName}"))))
                .ForMember(d => d.ProductionYear, o => o.MapFrom(s => s.ProductionYear))
                .ForMember(d => d.ProductionCountries, o => o.MapFrom(s => string.Join(", ", s.Countries!.Select(t => t.Description))))
                .ForMember(d => d.Duration, o => o.MapFrom(s => s.DurationMinutes))
                .ForMember(d => d.RightsStr, o => o.MapFrom(w => ConvertToRightsString(w.Rights.First())))
                ;

            CreateMap<Core.Entities.Works, EGEDARow>()
                .ForMember(d => d.CompactRef, o => o.MapFrom(s => s.CompactRef !=null ? $"CC-{s.CompactRef}" : "n/a") )
                .ForMember(d => d.TitleLanguages, o => o.MapFrom(s => string.Join(",", s.Titles.Select(t => t.LanguageCode))))
                .ForMember(d => d.Titles, o => o.MapFrom(s => string.Join(",", s.Titles.Select(t => t.Title))))
                .ForMember(d => d.SeasonNo, o => o.MapFrom(w => w.Discriminator == "Season" ? (w.Number > 0 ? w.Number : 0) : 0))
                .ForMember(d => d.EpisodeNo, o => o.MapFrom(w => w.Discriminator == "Episode" ? (w.Number > 0 ? w.Number : 0) : 0))
                .ForMember(d => d.Duration, o => o.MapFrom(s => s.DurationMinutes))
                .ForMember(d => d.WorkType, o => o.MapFrom(s => s.WorksType.Name))
                .ForMember(d => d.Genre, o => o.MapFrom(s => s.Genre.Name))
                .ForMember(d => d.YearOfProd, o => o.MapFrom(s => s.ProductionYear))
                .ForMember(d => d.FirstShowing, o => o.MapFrom(s => s.FirstBroadcastYear))
                .ForMember(d => d.ISANNo, o => o.MapFrom(s => s.Isan))
                .ForMember(d => d.Colour, o => o.MapFrom(s => "Yes"))
                .ForMember(d => d.BlackAndWhite, o => o.MapFrom(s => "No"))
                .ForMember(d => d.Silent, o => o.MapFrom(s => "No"))
                .ForMember(d => d.CountriesOfProduction, o => o.MapFrom(s => string.Join(", ", s.Countries!.Select(t => t.Description))))
                .ForMember(d => d.OriginalLanguages, o => o.MapFrom(s => string.Join(",", s.Languages.Select(l => l.Description))))
                .ForMember(d => d.Directors, o => o.MapFrom(s => string.Join(", ", s.Directors!.Select(t => $"{t.FirstName} {t.LastName}"))))
                .ForMember(d => d.Actors, o => o.MapFrom(s => string.Join(", ", s.Actors!.Select(t => $"{t.FirstName} {t.LastName}"))))
                .ForMember(d => d.Producers, o => o.MapFrom(s => string.Join(", ", s.Producers!.Select(t => $"{t.FirstName} {t.LastName}"))))
                .ForMember(d => d.Writers, o => o.MapFrom(s => string.Join(", ", s.ScreenWriters!.Select(t => $"{t.FirstName} {t.LastName}"))))
                .ForMember(d => d.ProductionCompanies, o => o.MapFrom(s => string.Join(", ", s.Companies!.Select(t => t.Name))))
                .ForMember(d => d.Percentage, o => o.MapFrom(s => s.Rights.First().Percentage))
                .ForMember(d => d.RightsFrom, o => o.MapFrom(s => s.Rights.First().StartOfRight.ToString("dd/MM/yyyy")))
                .ForMember(d => d.RightsTo, o => o.MapFrom(s => s.Rights.First().EndOfRight.ToString("dd/MM/yyyy")))
                ;

            CreateMap<Core.Entities.Works, GWFFRow>()
                .ForMember(d => d.CompactNo, o => o.MapFrom(s => s.CompactRef))
                .ForMember(d => d.Genre, o => o.MapFrom(s => s.Genre.Name))
                .ForMember(d => d.PeriodFrom, o => o.MapFrom(s => s.Rights.First().StartOfRight.ToString("dd/MM/yyyy")))
                .ForMember(d => d.PeriodTo, o => o.MapFrom(s => s.Rights.First().EndOfRight.ToString("dd/MM/yyyy")))
                .ForMember(d => d.OriginalTitle, o => o.MapFrom(s => s.Titles.Any(t => t.TitleType == TitleType.Main || t.TitleType == TitleType.Episode) 
                    ? s.Titles!.First(t => t.TitleType == TitleType.Main || t.TitleType == TitleType.Episode).Title : null))
                .ForMember(d => d.TitleOfEpisodes, o => o.MapFrom(s => s.Titles.Any(t => t.TitleType == TitleType.Episode) ? s.Titles!.First(t => t.TitleType == TitleType.Episode).Title : null))
                .ForMember(d => d.SeasonCount, o => o.MapFrom(w => w.Discriminator == "Season" ? (w.Number > 0 ? w.Number : null) : null))
                .ForMember(d => d.EpisodeCount, o => o.MapFrom(w => w.Discriminator == "Episode" ? (w.Number > 0 ? w.Number : null) : null))
                .ForMember(d => d.TypeOfWork, o => o.MapFrom(s => s.WorksType.Name))
                .ForMember(d => d.ProductionCompanies, o => o.MapFrom(s => string.Join(", ", s.Companies!.Select(t => t.Name))))
                .ForMember(d => d.Directors, o => o.MapFrom(s => string.Join(", ", s.Directors!.Select(t => $"{t.FirstName} {t.LastName}"))))
                .ForMember(d => d.Actors, o => o.MapFrom(s => string.Join(", ", s.Actors!.Select(t => $"{t.FirstName} {t.LastName}"))))
                .ForMember(d => d.YearOfProduction, o => o.MapFrom(s => s.ProductionYear))
                .ForMember(d => d.ProductionCountries, o => o.MapFrom(s => string.Join(", ", s.Countries!.Select(t => t.Description))))
                .ForMember(d => d.Duration, o => o.MapFrom(s => s.DurationMinutes))
                .ForMember(d => d.IsanNo, o => o.MapFrom(s => s.Isan))
                .ForMember(d => d.GermanTitle, o => o.MapFrom(s => s.Titles.Any(t => t.LanguageCode.ToUpper() == "GER") ? s.Titles.First(t => t.LanguageCode.ToUpper() == "GER").Title : null))
                .ForMember(d => d.AgicoaNo, o => o.MapFrom(s => s.AgicoaWorksReference))
                .ForMember(d => d.Percentage, o => o.MapFrom(s => s.Rights.First().Percentage))
                ;

            CreateMap<Core.Entities.Works, MPARow>()
                .ForMember(d => d.ReferenceId, o => o.MapFrom(s => s.CompactRef))
                .ForMember(d => d.CableNetwork, o => o.MapFrom(s => s.Rights.First().Percentage))
                .ForMember(d => d.CableSyndicated, o => o.MapFrom(s => s.Rights.First().Percentage))
                .ForMember(d => d.SatelliteNetwork, o => o.MapFrom(s => s.Rights.First().Percentage))
                .ForMember(d => d.SatelliteSyndicated, o => o.MapFrom(s => s.Rights.First().Percentage))
                .ForMember(d => d.Title, o => o.MapFrom<MPATitleConvertor>())
                .ForMember(d => d.Genre, o => o.MapFrom(s => s.Genre.Name))
                .ForMember(d => d.ProductionYear, o => o.MapFrom(s => s.ProductionYear))
                .ForMember(d => d.CountryIfNotUS, o => o.MapFrom(s => string.Join(", ", s.Countries!.Where(c => c.Code != "US").Select(t => t.Description))))
                .ForMember(d => d.DurationMinutes, o => o.MapFrom(s => s.DurationMinutes))
                .ForMember(d => d.ClaimStartDate, o => o.MapFrom(s => s.Rights.First().StartOfRight.ToString("dd/MM/yyyy")))
                .ForMember(d => d.ClaimEndDate, o => o.MapFrom(s => s.Rights.First().EndOfRight.ToString("dd/MM/yyyy")))
                .ForMember(d => d.Cast, o => o.MapFrom<CastConvertor>())
                .ForMember(d => d.Isan, o => o.MapFrom(s => s.Isan))
                ;


            CreateMap<Core.Entities.Works, UpfarArgoaRow>()
                .ForMember(d => d.SeriesOrStandAloneTitle, o => o.MapFrom<UpfarArgoaSeriesOrStandAloneTitleConvertor>())
                .ForMember(d => d.EpisodeTitle, o => o.MapFrom<UpfarArgoaEpisodeTitleConvertor>())
                .ForMember(d => d.SeasonTitle, o => o.MapFrom<UpfarArgoaSeasonTitleConvertor>())
                .ForMember(d => d.WorkType, o => o.MapFrom(s => s.WorksType!.Name))
                .ForMember(d => d.ProductionCountry, o => o.MapFrom(s => (s.Countries == null || !s.Countries.Any()) ? string.Empty : s.Countries.First().Code))
                .ForMember(d => d.Producer, o => o.MapFrom<ProducerCompanyConvertor>())
                .ForMember(d => d.Performer, o => o.MapFrom(s => string.Join(", ", s.Actors!.Select(t => $"{t.FirstName} {t.LastName}"))))
                .ForMember(d => d.QuotaRightsHeld, o => o.MapFrom<RightsConvertor>()) 
                .ForMember(d => d.Duration, o => o.MapFrom(s => s.DurationMinutes))
                .ForMember(d => d.YearOfCalculating, o => o.MapFrom(s => s.ProductionYear))
                .ForMember(d => d.IdentificationCode, o => o.MapFrom(s => s.CompactRef))
                .ForMember(d => d.ManagedRightsRetransmission, o => o.MapFrom(_ => "No"))
                .ForMember(d => d.ManagedRightsPrivate, o => o.MapFrom(_ => "Yes"))
                .ForMember(d => d.ManagedRightsPublic, o => o.MapFrom(_ => "Yes"))
                ;
        }

        public class UpfarArgoaSeriesOrStandAloneTitleConvertor : IValueResolver<Core.Entities.Works, UpfarArgoaRow, string?>
        {
            private readonly OscarContext _oscarContext;

            public UpfarArgoaSeriesOrStandAloneTitleConvertor(OscarContext oscarContext)
            {
                _oscarContext = oscarContext;
            }

            public string? Resolve(Core.Entities.Works works, UpfarArgoaRow destination,
                string? destMember,
                ResolutionContext context)
            {
                if (works.Discriminator == "Series" || works.Discriminator == "StandAlone")
                {
                    if (works.Titles.Any(t => t.TitleType == TitleType.Main))
                        return works.Titles!.First(t => t.TitleType == TitleType.Main)?.Title;
                    else
                        return null;
                }
                else 
                {
                    int? seriesId = null;
                    if (works.Discriminator == "Episode")
                    {
                        var episode = works as Core.Entities.Episode;
                        seriesId = episode.SeriesId;
                    }
                    else if (works.Discriminator == "Season")
                    {
                        var season = works as Core.Entities.Season;
                        seriesId = season.SeriesId;
                    }

                    if (seriesId > 0)
                    {
                        var series = _oscarContext
                            .Works
                            .AsNoTracking()
                            .Include(w => w.Titles.Where(t => t.TitleType == Core.Enums.TitleType.Main))
                            .SingleOrDefault(s => s.Id == seriesId);

                        if (series.Titles.Any(t => t.TitleType == TitleType.Main))
                            return series.Titles!.First(t => t.TitleType == TitleType.Main)?.Title;
                        else
                            return null;
                    }
                    else
                        return null;
                }
            }
        }

        public class UpfarArgoaEpisodeTitleConvertor : IValueResolver<Core.Entities.Works, UpfarArgoaRow, string?>
        {
            public string? Resolve(Core.Entities.Works works, UpfarArgoaRow destination,
                string? destMember,
                ResolutionContext context)
            {
                if (works.Discriminator == "Episode")
                {
                    if (works.Titles.Any(t => t.TitleType == TitleType.Main))
                        return works.Titles!.First(t => t.TitleType == TitleType.Main)?.Title;
                    else
                        return null;
                }
                else
                    return null;
            }
        }

        public class UpfarArgoaSeasonTitleConvertor : IValueResolver<Core.Entities.Works, UpfarArgoaRow, string?>
        {
            private readonly OscarContext _oscarContext;

            public UpfarArgoaSeasonTitleConvertor(OscarContext oscarContext)
            {
                _oscarContext = oscarContext;
            }

            public string? Resolve(Core.Entities.Works works, UpfarArgoaRow destination,
                string? destMember,
                ResolutionContext context)
            {
                if (works.Discriminator == "Season")
                {
                    if (works.Titles.Any(t => t.TitleType == TitleType.Main))
                        return works.Titles!.First(t => t.TitleType == TitleType.Main)?.Title;
                    else
                        return null;
                }
                else
                {
                    int? seasonId = null;
                    if (works.Discriminator == "Episode")
                    {
                        var episode = works as Core.Entities.Episode;
                        seasonId = episode.SeasonId;
                    }

                    if (seasonId > 0)
                    {
                        var season = _oscarContext
                            .Works
                            .AsNoTracking()
                            .Include(w => w.Titles.Where(t => t.TitleType == Core.Enums.TitleType.Main))
                            .SingleOrDefault(s => s.Id == seasonId);

                        if (season.Titles.Any(t => t.TitleType == TitleType.Main))
                            return season.Titles!.First(t => t.TitleType == TitleType.Main)?.Title;
                        else
                            return null;
                    }
                    else
                        return null;
                }
            }
        }

        public class EpisodeTitleConvertor : IValueResolver<Core.Entities.Works, CRCRow, string?>
        {
            private readonly OscarContext _oscarContext;

            public EpisodeTitleConvertor(OscarContext oscarContext)
            {
                _oscarContext = oscarContext;
            }

            public string? Resolve(Core.Entities.Works works, CRCRow destination,
                string? destMember,
                ResolutionContext context)
            {
                var title = works.Titles.Any(t => t.TitleType == TitleType.Episode) ? works.Titles!.First(t => t.TitleType == TitleType.Episode).Title : null;

                if (title == null)
                    title = works.Titles.Any(t => t.TitleType == TitleType.Main) ? works.Titles!.First(t => t.TitleType == TitleType.Main).Title : null;

                return title;
            }
        }

        public class SeasonTitleConvertor : IValueResolver<Core.Entities.Works, CRCRow, string?>
        {
            private readonly OscarContext _oscarContext;

            public SeasonTitleConvertor(OscarContext oscarContext)
            {
                _oscarContext = oscarContext;
            }

            public string? Resolve(Core.Entities.Works works, CRCRow destination,
                string? destMember,
                ResolutionContext context)
            {
                if (works.Discriminator != "Episode")
                {
                    if (works.Titles.Any(t => t.TitleType == TitleType.Main))
                        return works.Titles!.First(t => t.TitleType == TitleType.Main)?.Title;
                    else
                        return null;
                }

                var episode = works as Core.Entities.Episode;

                var season = _oscarContext
                    .Works
                    .AsNoTracking()
                    .Include(w => w.Titles.Where(t => t.TitleType == Core.Enums.TitleType.Main || t.TitleType == Core.Enums.TitleType.Episode))
                    .SingleOrDefault(s => s.Id == episode.SeasonId);

                return season?.Titles?.FirstOrDefault()?.Title;
            }
        }

        private string? ConvertToRightsString(Right r)
        {
            return $"{r.Type.Name}:{r.Percentage}:{r.StartOfRight.ToString("dd/MM/yyyy")}-{r.EndOfRight.ToString("dd/MM/yyyy")}:{r.Countries.First().Code}:{r.ChannelRights.First().Channel.Name}:{r.LanguageRights.First().Language.Name}";
        }


        public int? ConvertDiscriminator(String discriminator)
        {
            if (discriminator == null) return null;
            return discriminator switch
            {
                "Series" => 1,
                "Season" => 2,
                "Episode" => 3,
                _ => null
            };
        }

        public string? ConvertDiscriminatorToTitleType(String discriminator)
        {
            if (discriminator == null) return null;
            return discriminator switch
            {
                "Series" => "Series Header",
                "Season" => "Season Header",
                "Episode" => "Episode",
                "StandAlone" => "Title",
                _ => null
            };
        }

        public List<WorksInterestedPartiesScreenrightsExportDTO>? ConvertInterestedScreenrightsParties(IEnumerable<Oscar.Core.Entities.Actor> actors,
                                                                                     IEnumerable<Oscar.Core.Entities.Director> directors,
                                                                                     IEnumerable<Oscar.Core.Entities.Producer> producers,
                                                                                     IEnumerable<Oscar.Core.Entities.Distributor> distributors,
                                                                                     IEnumerable<Oscar.Core.Entities.ScriptWriter> scriptWriters,
                                                                                     IEnumerable<Oscar.Core.Entities.ScreenWriter> screenWriters)

        {
            var interestedPartiesScreenrightsExportDTOs = new List<WorksInterestedPartiesScreenrightsExportDTO>();

            var allPersonEntities = new List<PersonEntity>();
            if (actors != null) allPersonEntities.AddRange(actors);
            if (directors != null) allPersonEntities.AddRange(directors);
            if (producers != null) allPersonEntities.AddRange(producers);
            if (distributors != null) allPersonEntities.AddRange(distributors);
            if (scriptWriters != null) allPersonEntities.AddRange(scriptWriters);
            if (screenWriters != null) allPersonEntities.AddRange(screenWriters);

            foreach (var personEntity in allPersonEntities)
            {
                interestedPartiesScreenrightsExportDTOs.Add(
                    new WorksInterestedPartiesScreenrightsExportDTO()
                    {
                        Type = GetInterestedPartyType(personEntity),
                        Firstname = personEntity.FirstName,
                        LastName = personEntity.LastName
                    }

                );
            }

            return interestedPartiesScreenrightsExportDTOs;
        }

        public List<WorksInterestedPartiesAgicoaExportDTO>? ConvertInterestedAgicoaParties(IEnumerable<Oscar.Core.Entities.Actor> actors,
                                                                                     IEnumerable<Oscar.Core.Entities.Director> directors,
                                                                                     IEnumerable<Oscar.Core.Entities.Producer> producers,
                                                                                     IEnumerable<Oscar.Core.Entities.Distributor> distributors,
                                                                                     IEnumerable<Oscar.Core.Entities.ScriptWriter> scriptWriters,
                                                                                     IEnumerable<Oscar.Core.Entities.ScreenWriter> screenWriters)

        {
            var interestedPartiesAgicoaExportDTOs = new List<WorksInterestedPartiesAgicoaExportDTO>();

            var allPersonEntities = new List<PersonEntity>();
            if (actors != null) allPersonEntities.AddRange(actors);
            if (directors != null) allPersonEntities.AddRange(directors);
            if (producers != null) allPersonEntities.AddRange(producers);
            if (distributors != null) allPersonEntities.AddRange(distributors);
            if (scriptWriters != null) allPersonEntities.AddRange(scriptWriters);
            if (screenWriters != null) allPersonEntities.AddRange(screenWriters);

            foreach (var personEntity in allPersonEntities)
            {
                interestedPartiesAgicoaExportDTOs.Add(
                    new WorksInterestedPartiesAgicoaExportDTO()
                    {
                        Type = GetInterestedPartyType(personEntity),
                        Firstname = personEntity.FirstName,
                        LastName = personEntity.LastName
                    }

                );
            }

            return interestedPartiesAgicoaExportDTOs;
        }

        private int GetInterestedPartyType(PersonEntity personEntity)
        {
            return personEntity.GetType().Name switch
            {
                nameof(Actor) => 2,
                nameof(Director) => 1,
                nameof(Producer) => 3,
                nameof(Distributor) => 5,
                nameof(ScriptWriter) => 6,
                nameof(ScreenWriter) => 8,
                _ => -1
            };
        }

    }

    public class SerialNumberSenderScreenrightsConvertor : IValueResolver<Core.Entities.Works, RegistrationWorksScreenrightsExportDTO, string?>
    {
        private readonly OscarContext _oscarContext;

        public SerialNumberSenderScreenrightsConvertor(OscarContext oscarContext)
        {
            _oscarContext = oscarContext;
        }

        public string? Resolve(Core.Entities.Works works, RegistrationWorksScreenrightsExportDTO destination, string? destMember,
            ResolutionContext context)
        {
            if (works.Discriminator == "Series") return null;

            int? seriesId = null;

            switch (works.Discriminator)
            {
                case "Season":
                    {
                        var season = works as Core.Entities.Season;
                        seriesId = season.SeriesId;
                        break;
                    }
                case "Episode":
                    {
                        var episode = works as Core.Entities.Episode;
                        seriesId = episode.SeriesId;

                        if (seriesId == null)
                        {
                            var season = _oscarContext
                                .Seasons
                                .AsNoTracking()
                                .SingleOrDefault(s => s.Id == episode.SeasonId);

                            seriesId = season?.SeriesId;
                        }

                        break;
                    }
            }

            if (seriesId == null) return null;

            var series = _oscarContext
                .Works
                .AsNoTracking()
                .SingleOrDefault(s => s.Id == seriesId.Value);

            return series != null ? $"CC-{series.CompactRef}" : null;

        }
    }

    public class SerialNumberReceiverScreenrightsConvertor : IValueResolver<Core.Entities.Works, RegistrationWorksScreenrightsExportDTO, string?>
    {
        private readonly OscarContext _oscarContext;

        public SerialNumberReceiverScreenrightsConvertor(OscarContext oscarContext)
        {
            _oscarContext = oscarContext;
        }

        public string? Resolve(Core.Entities.Works works, RegistrationWorksScreenrightsExportDTO destination,
            string? destMember,
            ResolutionContext context)
        {
            if (works.Discriminator == "Series") return null;

            int? seriesId = null;
            var client = works.Clients!.FirstOrDefault();

            if (client == null) return null;

            switch (works.Discriminator)
            {
                case "Season":
                    {
                        var season = works as Core.Entities.Season;
                        seriesId = season.SeriesId;
                        break;
                    }
                case "Episode":
                    {
                        var episode = works as Core.Entities.Episode;
                        seriesId = episode.SeriesId;
                        if (seriesId == null)
                        {
                            var season = _oscarContext
                                .Seasons
                                .AsNoTracking()
                                .SingleOrDefault(s => s.Id == episode.SeasonId);

                            seriesId = season?.SeriesId;
                        }

                        break;
                    }
            }

            if (seriesId == null) return null;

            var series = _oscarContext
                .Works
                .AsNoTracking()
                .Include(c => c.ClientReferences!.Where(cr => cr.Client!.Id == client.Id))
                .SingleOrDefault(s => s.Id == seriesId.Value);

            return series != null &&
                   series.ClientReferences!.FirstOrDefault() != null &&
                   !string.IsNullOrEmpty(series.ClientReferences!.FirstOrDefault().AgicoaDeclarationNumber)
                ? series.ClientReferences!.FirstOrDefault()!.AgicoaDeclarationNumber
                : null;
        }
    }

    public class SeasonNumberScreenrightsConvertor : IValueResolver<Core.Entities.Works, RegistrationWorksScreenrightsExportDTO, string?>
    {
        private readonly OscarContext _oscarContext;

        public SeasonNumberScreenrightsConvertor(OscarContext oscarContext)
        {
            _oscarContext = oscarContext;
        }

        public string? Resolve(Core.Entities.Works works, RegistrationWorksScreenrightsExportDTO destination, string? destMember,
            ResolutionContext context)
        {
            if (works.Discriminator == "Season") return works.Number.ToString();

            if (works.Discriminator == "Episode")
            {
                var episode = works as Core.Entities.Episode;

                if (episode?.SeasonId == null) return null;

                var season = _oscarContext
                    .Works
                    .AsNoTracking()
                    .SingleOrDefault(s => s.Id == episode.SeasonId);

                if (season == null) return null;

                return season.Number.ToString();
            }

            return null;
        }
    }

    public class SeasonNumberSenderScreenrightsConvertor : IValueResolver<Core.Entities.Works, RegistrationWorksScreenrightsExportDTO, string?>
    {
        private readonly OscarContext _oscarContext;

        public SeasonNumberSenderScreenrightsConvertor(OscarContext oscarContext)
        {
            _oscarContext = oscarContext;
        }

        public string? Resolve(Core.Entities.Works works, RegistrationWorksScreenrightsExportDTO destination, string? destMember,
            ResolutionContext context)
        {
            if (works.Discriminator != "Episode") return null;
            var episode = works as Core.Entities.Episode;

            if (episode.SeasonId == null) return null;

            var season = _oscarContext
                .Works
                .AsNoTracking()
                .Include(c => c.ClientReferences)
                .SingleOrDefault(s => s.Id == episode.SeasonId);

            if (season == null) return null;

            return !string.IsNullOrEmpty(season.CompactRef) ? $"CC-{season.CompactRef}" : null;

        }
    }

    public class RightsholderReferenceSenderScreenrightsConvertor : IValueResolver<Core.Entities.Right, WorksRightScreenrightsExportDTO, string?>
    {
        public string? Resolve(Right right, WorksRightScreenrightsExportDTO destination, string? destMember, ResolutionContext context)
        {
            int iMaestroClientCode;
            bool isParsed = int.TryParse(right?.Client?.IMaestroClientCode, out iMaestroClientCode);
            return isParsed ? iMaestroClientCode.ToString() : right?.Client?.IMaestroClientCode;
        }
    }

    public class ServiceElectionScreenrightsConvertor : IValueResolver<Core.Entities.Right, WorksRightScreenrightsExportDTO, int>
    {
        public int Resolve(Core.Entities.Right right, WorksRightScreenrightsExportDTO destination, int destMember, ResolutionContext context)
        {
            var world = right.Countries.FirstOrDefault(x => x.Code == "*");
            if (world != null)
                return (int)ServiceElectionEnum.AllServicesAustraliaNZAndInternational;
            var australia = right.Countries.FirstOrDefault(x => x.Code == "AU");
            if (australia != null)
                return (int)ServiceElectionEnum.AllAustralianServices;
            var newZeaLand = right.Countries.FirstOrDefault(x => x.Code == "NZ");
            if (newZeaLand != null)
                return (int)ServiceElectionEnum.NewZealandEducationalCopying;
            return 0;
        }
    }

    public class SeasonNumberReceiverScreenrightsConvertor : IValueResolver<Core.Entities.Works, RegistrationWorksScreenrightsExportDTO, string?>
    {
        private readonly OscarContext _oscarContext;

        public SeasonNumberReceiverScreenrightsConvertor(OscarContext oscarContext)
        {
            _oscarContext = oscarContext;
        }

        public string? Resolve(Core.Entities.Works works, RegistrationWorksScreenrightsExportDTO destination,
            string? destMember,
            ResolutionContext context)
        {
            if (works.Discriminator != "Episode") return null;
            var episode = works as Core.Entities.Episode;

            var clientId = works.Clients?.FirstOrDefault()?.Id;

            var season = _oscarContext
                .Works
                .AsNoTracking()
                .Include(c => c.ClientReferences!.Where(cr => cr.Client!.Id == clientId))
                .SingleOrDefault(s => s.Id == episode.SeasonId);

            return season?.ClientReferences != null &&
                   season.ClientReferences.FirstOrDefault() != null &&
                   !string.IsNullOrEmpty(season.ClientReferences!.FirstOrDefault().AgicoaDeclarationNumber)
                ? season.ClientReferences.FirstOrDefault()!.AgicoaDeclarationNumber
                : null;
        }
    }

    public class SerialNumberSenderAgicoaConvertor: IValueResolver<Core.Entities.Works, RegistrationWorksAgicoaExportDTO, string?>
    {
        private readonly OscarContext _oscarContext;

        public SerialNumberSenderAgicoaConvertor(OscarContext oscarContext)
        {
            _oscarContext = oscarContext;
        }

        public string? Resolve(Core.Entities.Works works, RegistrationWorksAgicoaExportDTO destination, string? destMember,
            ResolutionContext context)
        {
            if (works.Discriminator == "Series") return null;

            int? seriesId = null;

            switch (works.Discriminator)
            {
                case "Season":
                {
                    var season = works as Core.Entities.Season;
                    seriesId = season.SeriesId;
                    break;
                }
                case "Episode":
                {
                    var episode = works as Core.Entities.Episode;
                    seriesId = episode.SeriesId;

                    if (seriesId == null)
                    {
                        var season = _oscarContext
                            .Seasons
                            .AsNoTracking()
                            .SingleOrDefault(s => s.Id == episode.SeasonId);

                        seriesId = season?.SeriesId;
                    }

                    break;
                }
            }

            if (seriesId == null) return null;

            var series = _oscarContext
                .Works
                .AsNoTracking()
                .SingleOrDefault(s => s.Id == seriesId.Value);

            return series != null ? $"CC-{series.CompactRef}" : null;

        }
    }

    public class SerialNumberReceiverAgicoaConvertor : IValueResolver<Core.Entities.Works, RegistrationWorksAgicoaExportDTO, string?>
    {
        private readonly OscarContext _oscarContext;

        public SerialNumberReceiverAgicoaConvertor(OscarContext oscarContext)
        {
            _oscarContext = oscarContext;
        }

        public string? Resolve(Core.Entities.Works works, RegistrationWorksAgicoaExportDTO destination,
            string? destMember,
            ResolutionContext context)
        {
            if (works.Discriminator == "Series") return null;

            int? seriesId = null;
            var client = works.Clients!.FirstOrDefault();

            if (client == null) return null;

            switch (works.Discriminator)
            {
                case "Season":
                {
                    var season = works as Core.Entities.Season;
                    seriesId = season.SeriesId;
                    break;
                }
                case "Episode":
                {
                    var episode = works as Core.Entities.Episode;
                    seriesId = episode.SeriesId;
                    if (seriesId == null)
                    {
                        var season = _oscarContext
                            .Seasons
                            .AsNoTracking()
                            .SingleOrDefault(s => s.Id == episode.SeasonId);

                        seriesId = season?.SeriesId;
                    }

                    break;
                }
            }

            if (seriesId == null) return null;

            var series = _oscarContext
                .Works
                .AsNoTracking()
                .Include(c => c.ClientReferences!.Where(cr => cr.Client!.Id == client.Id))
                .SingleOrDefault(s => s.Id == seriesId.Value);

            return series != null &&
                   series.ClientReferences!.FirstOrDefault() != null
                ? series.ClientReferences!.FirstOrDefault()!.AgicoaDeclarationNumber
                : null;
        }
    }

    public class SeasonNumberSenderAgicoaConvertor : IValueResolver<Core.Entities.Works, RegistrationWorksAgicoaExportDTO, string?>
    {
        private readonly OscarContext _oscarContext;

        public SeasonNumberSenderAgicoaConvertor(OscarContext oscarContext)
        {
            _oscarContext = oscarContext;
        }

        public string? Resolve(Core.Entities.Works works, RegistrationWorksAgicoaExportDTO destination, string? destMember,
            ResolutionContext context)
        {
            if (works.Discriminator != "Episode") return null;
            var episode = works as Core.Entities.Episode;

            if (episode.SeasonId == null) return null;

            var season = _oscarContext
                .Works
                .AsNoTracking()
                .Include(c => c.ClientReferences)
                .SingleOrDefault(s => s.Id == episode.SeasonId);

            if (season == null) return null;

            return !string.IsNullOrEmpty(season.CompactRef) ? $"CC-{season.CompactRef}" : null;

        }
    }

    public class SeasonNumberReceiverAgicoaConvertor : IValueResolver<Core.Entities.Works, RegistrationWorksAgicoaExportDTO, string?>
    {
        private readonly OscarContext _oscarContext;

        public SeasonNumberReceiverAgicoaConvertor(OscarContext oscarContext)
        {
            _oscarContext = oscarContext;
        }

        public string? Resolve(Core.Entities.Works works, RegistrationWorksAgicoaExportDTO destination,
            string? destMember,
            ResolutionContext context)
        {
            if (works.Discriminator != "Episode") return null;
            var episode = works as Core.Entities.Episode;

            var clientId = works.Clients?.FirstOrDefault()?.Id;

            var season = _oscarContext
                .Works
                .AsNoTracking()
                .Include(c => c.ClientReferences!.Where(cr => cr.Client!.Id == clientId))
                .SingleOrDefault(s => s.Id == episode.SeasonId);

            return season?.ClientReferences != null &&
                   season.ClientReferences.FirstOrDefault() != null
                ? season.ClientReferences.FirstOrDefault()!.AgicoaDeclarationNumber
                : null;
        }
    }

    public class PrincipalCastConvertor: IValueResolver<Core.Entities.Registration, CCCRow, string?>
    {
        public string? Resolve(Core.Entities.Registration source, CCCRow destination, string? destMember, ResolutionContext context)
        {
            var results = source.Works.Directors!.Select(a => a as PersonEntity).Concat(source.Works.Actors).Concat(source.Works.Producers!)
                .Concat(source.Works.ScreenWriters!).Concat(source.Works.ScriptWriters!);

            return string.Join(", ", results.Select(x => $"{x.FirstName} {x.LastName}").Take(3));
        }
    }

    public class CastConvertor : IValueResolver<Core.Entities.Works, MPARow, string?>
    {
        public string? Resolve(Core.Entities.Works source, MPARow destination, string? destMember, ResolutionContext context)
        {
            var results = source.Directors!.Select(a => a as PersonEntity).Concat(source.Actors).Concat(source.Producers!)
                .Concat(source.ScreenWriters!).Concat(source.ScriptWriters!);

            return string.Join(", ", results.Select(x => $"{x.FirstName} {x.LastName}").Take(3));
        }
    }

    public class RightsConvertor : IValueResolver<Core.Entities.Works, UpfarArgoaRow, string?>
    {
        public string? Resolve(Core.Entities.Works source, UpfarArgoaRow destination, string? destMember, ResolutionContext context)
        {
            if (source!.Rights == null)
                return string.Empty;
            else
                return $"{source!.Rights!.FirstOrDefault()?.Percentage:0}";
        }
    }

    public class ProducerCompanyConvertor : IValueResolver<Core.Entities.Works, UpfarArgoaRow, string?>
    {
        public string? Resolve(Core.Entities.Works source, UpfarArgoaRow destination, string? destMember, ResolutionContext context)
        {
            var producers = source.Producers!.Select(a => $"{a.FirstName} {a.LastName}");
            var companies = source.Companies.Select(c => c.Name);

            return string.Join(",", producers.Concat(companies));
        }
    }

    public class MPATitleConvertor : IValueResolver<Core.Entities.Works, MPARow, string?>
    {
        public string? Resolve(Core.Entities.Works source, MPARow destination, string? destMember, ResolutionContext context)
        {
            var title = source.Titles?.First(t => t.TitleType is TitleType.Main or TitleType.Episode).Title;

            if (title!.ToLower().StartsWith("the "))
                title = title.Remove(0, 4) + ", THE";

            if (title!.ToLower().StartsWith("a "))
                title = title.Remove(0, 2) + ", A";

            if (title!.ToLower().StartsWith("an "))
                title = title.Remove(0, 3) + ", AN";

            return title;
        }
    }
}