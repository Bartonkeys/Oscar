using AutoMapper;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;

namespace Oscar.Infrastructure.Features.Society.Mapping
{
    public partial class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SocietyDto, Core.Entities.Society>()
                .ForMember(d => d.Clients, o => o.Ignore())
                .ForMember(d => d.Contacts, o => o.Ignore())
                .ForMember(d => d.Addresses, o => o.Ignore())
                .ForMember(d => d.SocietyRights, o => o.Ignore())
                .ForMember(d => d.SocietyReferences, o => o.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Core.Entities.Society, SocietyDto>();

            CreateMap<SocietyRights, SocietyRightsDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<SocietyReference, SocietyReferenceDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.SocietyName, o => o.MapFrom(s => s.Society.Name))
                .ForMember(d => d.SocietyId, o => o.MapFrom(s => s.Society.Id))
                .ForMember(d => d.Reference, o => o.MapFrom(s => !string.IsNullOrEmpty(s.Reference) ? s.Reference : "Not Set"));

        }
    }
}
