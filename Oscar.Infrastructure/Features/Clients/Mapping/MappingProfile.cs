using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json.Serialization;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;

namespace Oscar.Infrastructure.Features.Clients.Mapping
{
    public partial class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ClientUpdateDto, Client>();

            CreateMap<ClientAddDto, Client>();

            CreateMap<ClientAddDto, Client>();

            CreateMap<ClientUpdateDto, Client>();

            CreateMap<ClientDto, Client>()
                .ForMember(d => d.Societies, o => o.Ignore())
                .ForMember(d => d.Rights, o => o.Ignore())
                .ForMember(d => d.Addresses, o => o.Ignore())
                .ForMember(d => d.Catalogues, o => o.Ignore())
                .ForMember(d => d.Contacts, o => o.Ignore());

            CreateMap<Client, ClientDto>()
                .ForMember(dest => dest.Contract, opt => opt.NullSubstitute(new Contract()))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Addresses.FirstOrDefault(x => x.IsCurrent == true)));

            CreateMap<Client, ClientBasicDto>();

            CreateMap<Contract, ContractDto>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<ClientAltName, ClientAltNameDto>().ReverseMap();
        }
    }
}
