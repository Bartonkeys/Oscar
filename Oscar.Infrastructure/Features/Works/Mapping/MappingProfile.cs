9*+
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Oscar.Core.DTOs;

namespace Oscar.Infrastructure.Features.Works.Mapping
{
    internal class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<Oscar.Core.Entities.WorksHeader, WorksDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.WorksId))
                .ForMember(d => d.Titles, o => o.MapFrom(s => new Collection<WorksTitleDto>() { new() { Title = s.Title } }))
                .ForMember(d => d.Clients, o => o.MapFrom(s => new Collection<ClientDto>() { new() { ClientName = s.ClientName } }))
                .ForMember(d => d.Catalogues, o => o.MapFrom(s => new Collection<CatalogueDto>() { new() { Name = s.CatalogueName ?? string.Empty } }))
                .ReverseMap();

            CreateMap<Oscar.Core.Entities.WorksTitle, WorksDto>()
                .ForMember(d => d.Discriminator, o => o.MapFrom(s => s.Works.Discriminator))
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Works.Id))
                .ForMember(d => d.CompactRef, o => o.MapFrom(s => s.Works.CompactRef))
                .ForMember(d => d.FirstBroadcastYear, o => o.MapFrom(s => s.Works.FirstBroadcastYear))
                .ForMember(d => d.ProductionYear, o => o.MapFrom(s => s.Works.ProductionYear))
                .ForMember(d => d.WorksStatus, o => o.MapFrom(s => s.Works.WorksStatus))
                .ForMember(d => d.Titles, o => o.MapFrom(s => new Collection<WorksTitleDto>() { new() { Title = s.Title, Id = s.Id} }))
                .ForMember(d => d.Clients, o => o.MapFrom(s => new Collection<ClientDto>() { new() { Id = s.Works.Clients.FirstOrDefault().Id, ClientName = s.Works.Clients.FirstOrDefault().ClientName } }))
                .ForMember(d => d.Catalogues, o => o.MapFrom(s => new Collection<CatalogueDto>() { new() { Id = s.Works.Catalogues.FirstOrDefault().Id, Name = s.Works.Catalogues.FirstOrDefault().Name }}))
                .ReverseMap();
        }
    }
}
