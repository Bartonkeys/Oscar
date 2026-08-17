using AutoMapper;
using Oscar.Core.Entities;
using Oscar.MRIT.Core.DTOs;
using Oscar.MRIT.Core.Enums;
using Oscar.MRIT.Core.MRITModels;

namespace Oscar.Mrit.Features.MRITIntegration.Mapping
{
    public partial class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<VwOnMusicFelixWorks, FelixWorksDto>();
            CreateMap<Oscar.Core.Entities.Works, WorksDto>();

            CreateMap<VwOnMusicFelixWorks, ProductionModel>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.WorksId))
                .ForMember(d => d.EnglishTitle, o => o.MapFrom(s => s.Titles))
                .ForMember(d => d.IsOneOff, o => o.MapFrom(s => s.SerialLevel == (int)SerialLevel.OneOff));

            CreateMap<VwOnMusicFelixWorks, EpisodeModel>();

            CreateMap<VwOnMusicFelixWorks, PersonModel>();
        }
    }
}
