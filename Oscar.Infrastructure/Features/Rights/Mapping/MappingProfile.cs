using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;

namespace Oscar.Infrastructure.Features.Rights.Mapping
{
    public partial class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Right, RightDto>().ReverseMap();
            CreateMap<Oscar.Core.Entities.Channel, ChannelDto>().ReverseMap();
            CreateMap<CountryChannelRights, CountryChannelRightsDto>().ReverseMap();
            CreateMap<ChannelRights, ChannelRightsDto>().ReverseMap();
            CreateMap<LanguageRights, LanguageRightsDto>().ReverseMap();
            CreateMap<Core.Entities.Catalogue, CatalogueDto>().ReverseMap();
        }
    }
}
