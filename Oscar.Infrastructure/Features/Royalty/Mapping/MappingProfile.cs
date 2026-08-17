using AutoMapper;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;

namespace Oscar.Infrastructure.Features.Royalty.Mapping;

public partial class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<MerlinSociety, MerlinSocietyDto>().ReverseMap()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }


}

