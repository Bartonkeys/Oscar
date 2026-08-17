using AutoMapper;
using Oscar.MRIT.Core.DTOs;
using Oscar.Mrit.Data;

namespace Oscar.Mrit.Features.FelixMrit.Mapping
{
    public partial class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<FelixMritMatchDto, Match>()
                .ForMember(d => d.Works, o => o.MapFrom<WorksResolver>())
                .ForMember(d => d.Companies, o => o.MapFrom<CompanyResolver>())
                //.ForMember(d => d.AltProductionTitles, o => o.MapFrom(d => d.AltProductionTitles.Select(p => new AltProductionTitle{Name = p})))
                //.ForMember(d => d.AltRecordTitles, o => o.MapFrom(d => d.AltRecordTitles.Select(r => new AltRecordTitle{Name = r})))
                .ForMember(d => d.AltProductionTitles, o => o.MapFrom<AltProductionTitleResolver>())
                .ForMember(d => d.AltRecordTitles, o => o.MapFrom<AltRecordTitleResolver>())
                .ForMember(d => d.Countries, o => o.MapFrom<CountryResolver>())
                .ForMember(d => d.Genres, o => o.MapFrom<GenreResolver>())
                .ForMember(d => d.Languages, o => o.MapFrom<LanguageResolver>())
                .ForMember(d => d.PersonOfInterests, o => o.MapFrom<PersonOfInterestResolver>())
                .ForMember(d => d.BatchJob, o => o.MapFrom<BatchJobResolver>());

            CreateMap<TransmissionDto, Transmission>()
                .ForMember(d => d.Territories, o => o.MapFrom<TerritoryResolver>());
        }
    }
}
