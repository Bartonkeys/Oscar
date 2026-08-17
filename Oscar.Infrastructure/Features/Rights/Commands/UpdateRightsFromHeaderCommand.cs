using AutoMapper;
using BartonKeys.Functional;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Rights.Queries;

namespace Oscar.Infrastructure.Features.Rights.Commands
{
    public class UpdateRightsFromHeaderCommand: IRequest<Result>
    {
        public int Id { get; set; }
        public List<RightDto>? Rights { get; set; }
        public Discriminator Discriminator { get; set; }
    }

    public class UpdateRightsFromHeaderCommandHandler: SimpleAbstractBaseHandler<UpdateRightsFromHeaderCommand>
    {
        private readonly IMediator _mediator;

        public UpdateRightsFromHeaderCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<UpdateRightsFromHeaderCommand> validator, 
            ILogger<UpdateRightsFromHeaderCommand> logger, IMediator mediator) : base(oscarContext, mapper, validator, logger)
        {
            _mediator = mediator;
        }

        protected override async Task<Result> HandleRequest(UpdateRightsFromHeaderCommand request, CancellationToken cancellationToken)
        {
            var worksRights = await _mediator.Send(new GetRightsByWorksIdQuery
            {
                WorksId = request.Id,
            });

            if (worksRights.IsFailure || !worksRights.Value.Any())
                return Result.Fail("Works has no rights assigned");

            var deleteResult = await _mediator.Send(new DeleteRightsFromHeaderCommand
            {
                Id = request.Id,
                Discriminator = request.Discriminator
            }, cancellationToken);

            if (deleteResult.IsFailure) return deleteResult;

            switch (request.Discriminator)
            {
                case Discriminator.Series:
                    await UpdateSeasonAndEpisodeRights(request.Id, worksRights.Value, cancellationToken);
                    break;
                case Discriminator.Season:
                    await UpdateEpisodeRights(request.Id, worksRights.Value, cancellationToken);
                    break;
                case Discriminator.Episode:
                case Discriminator.StandAlone:
                case Discriminator.All:
                default:
                    break;
            }

            return Result.Ok();
        }

        private async Task UpdateSeasonAndEpisodeRights(int worksId, IEnumerable<RightDto> worksRights, CancellationToken cancellationToken)
        {
            var series = OscarContext.Series
                .Include(s => s.Seasons)!.ThenInclude(s => s.Episodes)!.ThenInclude(r => r.Rights)
                .Include(s => s.Seasons)!.ThenInclude(r => r.Rights)
                .Include(s => s.Clients)
                .Include(c => c.Catalogues)
                .AsSplitQuery()
                .Single(s => s.Id == worksId);

            var catalogue = series.Catalogues.First();
            var client = series.Clients.First();
            
            foreach (var season in series.Seasons!)
            {
                season.Rights = CloneRights(worksRights, series, client, catalogue).ToList();

                foreach (var episode in season.Episodes!) 
                    episode.Rights = CloneRights(worksRights, episode, client, catalogue).ToList();
            }

            await OscarContext.SaveChangesAsync(cancellationToken);
        }

        private async Task UpdateEpisodeRights(int requestId, IEnumerable<RightDto> worksRights, CancellationToken cancellationToken)
        {
            var season = OscarContext.Seasons
                .Include(s => s.Episodes)!.ThenInclude(r => r.Rights)
                .Include(s => s.Clients)
                .Include(c => c.Catalogues)
                .AsSplitQuery()
                .Single(s => s.Id == requestId);

            var catalogue = season.Catalogues.First();
            var client = season.Clients.First();

            foreach (var episode in season.Episodes!)
                episode.Rights = CloneRights(worksRights, episode, client, catalogue).ToList();

            await OscarContext.SaveChangesAsync(cancellationToken);
        }

        private IEnumerable<Right> CloneRights(IEnumerable<RightDto> worksRights, Core.Entities.Works works, Client client, Core.Entities.Catalogue catalogue)
        {
            foreach (var worksRight in worksRights)
            {
                var right = new Right
                {
                    Type = OscarContext.RightsTypes.Single(r => r.Id == worksRight.Type.Id),
                    Client = client,
                    StartOfRight = works.ProductionYear != null ? new DateTime(works.ProductionYear.Value, 1, 1) : worksRight.StartOfRight,
                    EndOfRight = worksRight.EndOfRight,
                    StartOfValidity = works.ProductionYear != null ? new DateTime(works.ProductionYear.Value, 1, 1) : worksRight.StartOfValidity,
                    EndOfValidity = worksRight.EndOfValidity,
                    Notations = worksRight.Notations,
                    Percentage = worksRight.Percentage,
                    CreationDate = DateTime.Now,
                    Work = works,
                    Countries = new List<Core.Entities.Country>(),
                    Catalogue = catalogue
                };

                RightsHelper.SetChannelRights(right, worksRight.ChannelRights.Select(cr => cr.Channel.Id).ToList(), OscarContext);
                RightsHelper.SetLanguageRights(right, worksRight.LanguageRights.Select(l => l.Language.Id).ToList(), OscarContext);
                RightsHelper.SetCollection(right.Countries, worksRight.Countries.Select(c => c.Id).ToList(), OscarContext);

                yield return right;
            }
        }
    }
}
