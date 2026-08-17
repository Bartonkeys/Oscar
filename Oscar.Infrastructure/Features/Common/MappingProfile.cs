using AutoMapper;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;

namespace Oscar.Infrastructure.Features.Common
{
    public partial class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Core.Entities.Works, Core.Entities.Series>();
            CreateMap<Core.Entities.Works, Core.Entities.Season>();
            CreateMap<Core.Entities.Works, Core.Entities.StandAlone>();
            CreateMap<Core.Entities.Works, Core.Entities.Episode>();

            CreateMap<WorksImportDto, Core.Entities.WorksImport>().ReverseMap();
            CreateMap<WorksRightsImportDto, Core.Entities.WorksRightsImport>().ReverseMap();

            CreateMap<WorksImportRequestAddDto, WorksImportRequest>();
            CreateMap<WorksImportRequestDto, WorksImportRequest>().ReverseMap()
                .ForMember(d => d.ClientName, o => o.MapFrom(s => s.Client.ClientName))
                .ForMember(d => d.CatalogueName, o => o.MapFrom(s => s.Catalogue != null ? s.Catalogue.Name : null));

            CreateMap<MatchRequest,MatchResultDto>();
            CreateMap<MatchTemplateDto, MatchTemplateResultsDto>();

            CreateMap<MatchRequestAddDto, MatchRequest>()
                .ForMember(d => d.RightsTypeId, o=> o.MapFrom(s => s.RightsType!.Id));

            CreateMap<MatchRequestDto, MatchRequest>().ReverseMap(); ;

            CreateMap<EpisodeUpdateDto, Core.Entities.Episode>();
            CreateMap<EpisodeAddDto, Core.Entities.Episode>();

            CreateMap<SeasonUpdateDto, Core.Entities.Season>();
            CreateMap<SeasonAddDto, Core.Entities.Season>();

            CreateMap<StandAloneUpdateDto, Core.Entities.StandAlone>();
            CreateMap<StandAloneAddDto, Core.Entities.StandAlone>();

            CreateMap<SeriesUpdateDto, Core.Entities.Series>();
            CreateMap<SeriesAddDto, Core.Entities.Series>();

            CreateMap<Core.Entities.Country, Core.DTOs.CountryDto>().ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<AddressDto, Address>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<AddressAddDto, Address>().
                ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Address, AddressDto>();

            CreateMap<Core.Entities.StandAlone, StandAloneDto>().ReverseMap();
            CreateMap<Core.Entities.Episode, EpisodeDto>().ReverseMap();
            CreateMap<Core.Entities.Series, SeriesDto>().ReverseMap();
            CreateMap<Core.Entities.Season, SeasonDto>().ReverseMap();
            CreateMap<WorksDto, Oscar.Core.Entities.Works>().ReverseMap();

            CreateMap<ClientReference, ClientReferenceDto>()
                .ForMember(d => d.ClientName, o => o.MapFrom(s => s.Client.ClientName))
                .ForMember(d => d.ClientId, o => o.MapFrom(s => s.Client.Id))
                .ForMember(d => d.RHId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.AgicoaDeclarationNumber, o => o.MapFrom(s => s.AgicoaDeclarationNumber));

            CreateMap<Core.Entities.Report, ReportDto>().ReverseMap();
            CreateMap<ReportField, ReportFieldDto>().ReverseMap();

            CreateMap<AlternativeTitle, AlternativeTitleDto>().ReverseMap();
            CreateMap<CatalogueDto, Oscar.Core.Entities.Catalogue>().ReverseMap()
                //.ForMember(d => d.Societies, o => o.MapFrom(s => s.SocietyReferences != null && s.SocietyReferences.Select(t => t.Society) != null && s.SocietyReferences.Select(t => t.Society).Count() > 0 ? s.SocietyReferences.Select(t => t.Society) : null));
                .ForMember(d => d.Societies, o => o.MapFrom(s => s.Client != null && s.Client.Societies != null && s.Client.Societies.Count() > 0 ? s.Client.Societies : null));

            CreateMap<Core.Entities.Catalogue, CatalogueAddDto>().ReverseMap();
            CreateMap<Company, CompanyDto>().ReverseMap();
            CreateMap<CompanyAddDto, Core.Entities.Company>();
            CreateMap<Core.Entities.Conflict, ConflictDto>().ReverseMap();
            CreateMap<Genre, GenreDto>().ReverseMap();
            CreateMap<GenreSubType, GenreSubTypeDto>().ReverseMap();
            CreateMap<WorksSubType, WorksSubTypeDto>().ReverseMap();
            CreateMap<Language, LanguageDto>().ReverseMap();
            CreateMap<Mandate, MandateDto>().ReverseMap(); 
            CreateMap<MandateType, MandateTypeDto>().ReverseMap();
            CreateMap<LookUpEntity, LookUpDto>().ReverseMap();
            CreateMap<OtherName, OtherNameDto>().ReverseMap();
            CreateMap<PersonEntity, PersonDto>().ReverseMap();
            CreateMap<Oscar.Core.Entities.Producer, ProducerDto>().ReverseMap();
            CreateMap<Oscar.Core.Entities.Director, DirectorDto>().ReverseMap();
            CreateMap<Oscar.Core.Entities.Actor, ActorDto>().ReverseMap();
            CreateMap<Oscar.Core.Entities.Distributor, DistributorDto>().ReverseMap();
            CreateMap<Oscar.Core.Entities.ScreenWriter, ScreenWriterDto>().ReverseMap();
            CreateMap<Oscar.Core.Entities.ScriptWriter, ScriptWriterDto>().ReverseMap();
            CreateMap<WorksStatus, WorksStatusDto>().ReverseMap();
            CreateMap<WorksStatusHistory, WorksStatusHistoryDto>().ReverseMap();
            CreateMap<WorksType, WorksTypeDto>().ReverseMap();
            CreateMap<WorksTitle, WorksTitleDto>().ReverseMap();
            CreateMap<Right, RightDto>().ReverseMap()
                .ForMember(r => r.Work, o => o.MapFrom(s => s.Work != null ? s.Work : null));

            CreateMap<RightsType, RightsTypeDto>().ReverseMap();

            CreateMap<RegistrationBatch, RegistrationBatchCreateDto>().ReverseMap();
            CreateMap<Oscar.Core.Entities.Registration, RegistrationCreateDto>().ReverseMap();
            CreateMap<RegistrationDisplayDto, Oscar.Core.Entities.Registration>().ReverseMap()
               .ForMember(d => d.ClientName, o => o.MapFrom(s => s.Client != null ? s.Client.ClientName : null))
               .ForMember(d => d.CatalogueName, o => o.MapFrom(s => s.Catalogue != null ? s.Catalogue.Name : null))
               .ForMember(d => d.SocietyName, o => o.MapFrom(s => s.Society != null ? s.Society.Name : null))
               .ForMember(d => d.Titles, o => o.MapFrom(s => s.Works != null && s.Works.Titles != null && s.Works.Titles.Count() > 0 ? s.Works.Titles : null))
               .ForMember(d => d.RegistrationBatch, o => o.MapFrom(s => s.RegistrationBatch));
            CreateMap<RegistrationBatchDisplayDto, Oscar.Core.Entities.RegistrationBatch>().ReverseMap();
            CreateMap<Oscar.Core.Entities.ReRegistration, ReRegistrationDto>().ReverseMap();

            CreateMap<Oscar.Core.Entities.Producer, PersonDto>().ReverseMap();
            CreateMap<Oscar.Core.Entities.Director, PersonDto>().ReverseMap();
            CreateMap<Oscar.Core.Entities.Actor, PersonDto>().ReverseMap();
            CreateMap<Oscar.Core.Entities.Distributor, PersonDto>().ReverseMap();
            CreateMap<Oscar.Core.Entities.ScreenWriter, PersonDto>().ReverseMap();
            CreateMap<Oscar.Core.Entities.ScriptWriter, PersonDto>().ReverseMap();

            CreateMap<Oscar.Core.Entities.Actor, ActorDto>().ReverseMap();
            CreateMap<Oscar.Core.Entities.CustomerServiceManager, CustomerServiceManagerDto>().ReverseMap();
            CreateMap<Oscar.Core.Entities.Operator, OperatorDto>().ReverseMap();

            CreateMap<Right, Right>()
                .ForMember(d => d.Id, o => o.Ignore());

            CreateMap<Core.Entities.Series, DuplicateDto>()
               .ForMember(d => d.Client, o => o.MapFrom(s => s.Clients != null && s.Clients.Count() > 0 ? s.Clients.First().ClientName : ""))
               .ForMember(d => d.Catalogue, o => o.MapFrom(s => s.Catalogues != null && s.Catalogues.Count() > 0 ? s.Catalogues.First().Name : ""))
               .ForMember(d => d.Title, o => o.MapFrom(s => s.Titles != null && s.Titles.Count() > 0 ? s.Titles.First().Title : ""));

            CreateMap<Oscar.Core.Entities.EquivalenceRequest, EquivalenceRequestDto>().ReverseMap();
            CreateMap<Oscar.Core.Entities.Document, DocumentDto>().ReverseMap();
            CreateMap<Oscar.Core.Entities.ScreenrightsRequest, ScreenrightsRequestDto>().ReverseMap();



        }
    }
}
