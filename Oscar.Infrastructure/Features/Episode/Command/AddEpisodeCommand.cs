using System.Globalization;
using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Rights.Commands;
using Oscar.Infrastructure.Features.Rights.Queries;

namespace Oscar.Infrastructure.Features.Episode.Commands
{
    public class AddEpisodeCommand: IRequest<Result<EpisodeDto>>
    {
        public EpisodeAddDto EpisodeAddDto { get; set; }
    }

    public class AddEpisodeCommandHandler : AbstractBaseHandler<AddEpisodeCommand, EpisodeDto>
    {
        private readonly IMediator _mediator;

        public AddEpisodeCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<AddEpisodeCommand> validator, 
            ILogger<AddEpisodeCommand> logger, IMediator mediator) : base(oscarContext, mapper, validator, logger)
        {
            _mediator = mediator;
        }

        protected override async Task<Result<EpisodeDto>> HandleRequest(AddEpisodeCommand request, CancellationToken cancellationToken)
        {
            foreach (var item in request.EpisodeAddDto.Titles.Where(t => t.Id < 0))
                item.Id = 0;

            var episode = Mapper.Map<Core.Entities.Episode>(request.EpisodeAddDto);
            episode.Countries = new List<Core.Entities.Country>();
            episode.Directors = new List<Core.Entities.Director>();
            episode.Languages = new List<Core.Entities.Language>();
            episode.Actors = new List<Core.Entities.Actor>();
            episode.Producers = new List<Core.Entities.Producer>();
            episode.Companies = new List<Core.Entities.Company>();
            episode.Mandates = new List<Core.Entities.Mandate>();
            WorksHelper.SetCollection<Core.Entities.Country>(episode.Countries, request.EpisodeAddDto.CountryIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Director>(episode.Directors, request.EpisodeAddDto.DirectorIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Language>(episode.Languages, request.EpisodeAddDto.LanguageIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Actor>(episode.Actors, request.EpisodeAddDto.ActorIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Producer>(episode.Producers, request.EpisodeAddDto.ProducerIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Company>(episode.Companies, request.EpisodeAddDto.CompanyIds, OscarContext);
            WorksHelper.SetMandates(episode.Mandates, request.EpisodeAddDto.MandateTypes, OscarContext);

            await AssignClientAndCatalogue(episode, request.EpisodeAddDto.SeasonId, cancellationToken);

            episode.CompactRef = AutoGenerateCompactRef();

            episode.ClientReferences = new List<ClientReference>() { new ClientReference() };

            OscarContext.Add(episode);
            await OscarContext.SaveChangesAsync(cancellationToken);

            await InheritRights(episode);

            Logger.LogInformation((int)EpisodeFeatureEvent.Add, CommandResult.SUCCESS);
            return Result.Ok(Mapper.Map<EpisodeDto>(episode));
        }

        private async Task AssignClientAndCatalogue(Core.Entities.Episode episode, int? seasonId, CancellationToken cancellationToken)
        {
            var season = await OscarContext.Seasons
                .Include(i => i.Clients)
                .Include(i => i.Catalogues)
                .AsSplitQuery()
                .SingleOrDefaultAsync(w => w.Id == seasonId, cancellationToken);

            episode.Clients = season.Clients;
            episode.Catalogues = season.Catalogues;
        }

        private async Task InheritRights(Core.Entities.Episode episode)
        {
            if (episode.Clients == null || episode.Clients.Count == 0)
                return;

            var inheritedWorksRights = (await _mediator.Send(new GetRightsByClientIdQuery
                {
                    ClientId = episode.Clients.First().Id,
                    CatalogueId = episode.Catalogues != null && episode.Catalogues.Any() ? episode.Catalogues.First().Id : null
                }
            )).Value;

            if (episode.Catalogues == null || !episode.Catalogues.Any())
                inheritedWorksRights = inheritedWorksRights.GetClientOnlyRights();

            foreach (var inheritedWorksRight in inheritedWorksRights)
            {
                await _mediator.Send(new AddRightCommand
                {
                    RightAddDto = new RightAddDto
                    {
                        CatalogueID = inheritedWorksRight.Catalogue?.Id,
                        ChannelIds = inheritedWorksRight.ChannelRights.Select(c => c.Channel.Id).ToList(),
                        ClientID = episode.Clients.First().Id,
                        CountryIds = inheritedWorksRight.Countries.Select(c => c.Id).ToList(),
                        Start = inheritedWorksRight.StartOfRight,
                        End = inheritedWorksRight.EndOfRight,
                        StartValidity = inheritedWorksRight.StartOfValidity,
                        EndValidity = inheritedWorksRight.EndOfValidity,
                        TypeID = inheritedWorksRight.TypeId,
                        Percentage = inheritedWorksRight.Percentage,
                        Notations = inheritedWorksRight.Notations,
                        WorksID = episode.Id,
                        LanguageIds = inheritedWorksRight.LanguageRights.Select(l => l.Language.Id).ToList()
                    }
                });
            }
        }
    }
}
