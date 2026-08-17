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
using System.Threading;

namespace Oscar.Infrastructure.Features.Rights.Commands
{
    public class DeleteRightsFromHeaderCommand : IRequest<Result>
    {
        public int Id { get; set; }
        public Discriminator Discriminator { get; set; }    
    }

    public class DeleteRightsFromHeaderCommandHandler : SimpleAbstractBaseHandler<DeleteRightsFromHeaderCommand>
    {
        private readonly IMediator _mediator;

        public DeleteRightsFromHeaderCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<DeleteRightsFromHeaderCommand> validator, ILogger<DeleteRightsFromHeaderCommand> logger, IMediator mediator) 
            : base(oscarContext, mapper, validator, logger)
        {
            _mediator = mediator;
        }

        protected override async Task<Result> HandleRequest(DeleteRightsFromHeaderCommand request, CancellationToken cancellationToken)
        {
            switch (request.Discriminator)
            {
                case Discriminator.Series:
                    await DeleteSeasonAndEpisodeRights(request.Id, cancellationToken);
                    break;
                case Discriminator.Season:
                    await DeleteEpisodeRights(request.Id, cancellationToken);
                    break;
                case Discriminator.Episode:
                case Discriminator.StandAlone:
                case Discriminator.All:
                default:
                    break;
            }

            return Result.Ok();
        }

        private async Task DeleteSeasonAndEpisodeRights(int requestId, CancellationToken cancellationToken)
        {
            var series = OscarContext.Series
                .Include(s => s.Seasons)!.ThenInclude(s => s.Episodes)!.ThenInclude(r => r.Rights)
                .Include(s => s.Seasons)!.ThenInclude(r => r.Rights)
                .AsSplitQuery()
                .Single(s => s.Id == requestId);

            foreach (var season in series.Seasons!)
            {
                foreach (var seasonRight in season.Rights)
                    await DeleteRight(seasonRight.Id, cancellationToken);

                foreach (var episode in season.Episodes!)
                {
                    foreach (var epsiodeRight in episode.Rights)
                        await DeleteRight(epsiodeRight.Id, cancellationToken);
                }
            }
        }

        private async Task DeleteEpisodeRights(int requestId, CancellationToken cancellationToken)
        {
            var season = OscarContext.Seasons
                .Include(s => s.Episodes)!.ThenInclude(r => r.Rights)
                .AsSplitQuery()
                .Single(s => s.Id == requestId);

            foreach (var episode in season.Episodes!)
            {
                foreach (var epsiodeRight in episode.Rights)
                    await DeleteRight(epsiodeRight.Id, cancellationToken);
            }
        }

        private async Task DeleteRight(int id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteRightCommand
            {
                RightDeleteDto = new RightDeleteDto { ID = id }
            }, cancellationToken);
        }
    }
}
