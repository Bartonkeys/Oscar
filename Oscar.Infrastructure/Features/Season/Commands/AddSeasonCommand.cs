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
using Oscar.Infrastructure.Features.Series.Queries;

namespace Oscar.Infrastructure.Features.Season.Commands
{
    public class AddSeasonCommand: IRequest<Result<SeasonDto>>
    {
        public SeasonAddDto SeasonAddDto { get; set; }
    }

    public class AddSeasonCommandHandler : AbstractBaseHandler<AddSeasonCommand, SeasonDto>
    {
        private readonly IMediator _mediator;

        public AddSeasonCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<AddSeasonCommand> validator, 
            ILogger<AddSeasonCommand> logger, IMediator mediator) : base(oscarContext, mapper, validator, logger)
        {
            _mediator = mediator;
        }

        protected override async Task<Result<SeasonDto>> HandleRequest(AddSeasonCommand request, CancellationToken cancellationToken)
        {
            foreach (var item in request.SeasonAddDto.Titles.Where(t => t.Id < 0))
                item.Id = 0;

            var season = Mapper.Map<Core.Entities.Season>(request.SeasonAddDto);
            season.Countries = new List<Core.Entities.Country>();
            season.Directors = new List<Core.Entities.Director>();
            season.Languages = new List<Core.Entities.Language>();
            season.Actors = new List<Core.Entities.Actor>();
            season.Producers = new List<Core.Entities.Producer>();
            season.Companies = new List<Core.Entities.Company>();
            season.Mandates = new List<Core.Entities.Mandate>();

            WorksHelper.SetCollection<Core.Entities.Country>(season.Countries, request.SeasonAddDto.CountryIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Director>(season.Directors, request.SeasonAddDto.DirectorIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Language>(season.Languages, request.SeasonAddDto.LanguageIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Actor>(season.Actors, request.SeasonAddDto.ActorIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Producer>(season.Producers, request.SeasonAddDto.ProducerIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Company>(season.Companies, request.SeasonAddDto.CompanyIds, OscarContext);
            WorksHelper.SetMandates(season.Mandates, request.SeasonAddDto.MandateTypes, OscarContext);

            await AssignClientAndCatalogue(season, request.SeasonAddDto.SeriesId, cancellationToken);

            season.CompactRef = AutoGenerateCompactRef();

            season.ClientReferences = new List<ClientReference>() { new ClientReference() };

            OscarContext.Add(season);
            await OscarContext.SaveChangesAsync(cancellationToken);

            await InheritRights(season);

            Logger.LogInformation((int)SeasonFeatureEvent.Add, CommandResult.SUCCESS);
            return Result.Ok(Mapper.Map<SeasonDto>(season));
        }

        private async Task AssignClientAndCatalogue(Core.Entities.Season season, int? seriesId, CancellationToken cancellationToken)
        {
            var series = await OscarContext.Series
                .Include(i => i.Clients)
                .Include(i => i.Catalogues)
                .AsSplitQuery()
                .SingleOrDefaultAsync(w => w.Id == seriesId, cancellationToken);

            season.Clients = series.Clients;
            season.Catalogues = series.Catalogues;
        }

        private async Task InheritRights(Core.Entities.Season season)
        {
            if (season.Clients == null || season.Clients.Count == 0)
                return;

            var inheritedWorksRights = (await _mediator.Send(new GetRightsByClientIdQuery
                {
                    ClientId = season.Clients.First().Id,
                    CatalogueId = season.Catalogues != null && season.Catalogues.Any() ? season.Catalogues.First().Id : null
                }
            )).Value;

            if (season.Catalogues == null || !season.Catalogues.Any())
                inheritedWorksRights = inheritedWorksRights.GetClientOnlyRights();

            foreach (var inheritedWorksRight in inheritedWorksRights)
            {
                await _mediator.Send(new AddRightCommand
                {
                    RightAddDto = new RightAddDto
                    {
                        CatalogueID = inheritedWorksRight.Catalogue?.Id,
                        ChannelIds = inheritedWorksRight.ChannelRights.Select(c => c.Channel.Id).ToList(),
                        ClientID = season.Clients.First().Id,
                        CountryIds = inheritedWorksRight.Countries.Select(c => c.Id).ToList(),
                        Start = inheritedWorksRight.StartOfRight,
                        End = inheritedWorksRight.EndOfRight,
                        StartValidity = inheritedWorksRight.StartOfValidity,
                        EndValidity = inheritedWorksRight.EndOfValidity,
                        TypeID = inheritedWorksRight.TypeId,
                        Percentage = inheritedWorksRight.Percentage,
                        Notations = inheritedWorksRight.Notations,
                        WorksID = season.Id,
                        LanguageIds = inheritedWorksRight.LanguageRights.Select(l => l.Language.Id).ToList()
                    }
                });
            }
        }
    }
}
