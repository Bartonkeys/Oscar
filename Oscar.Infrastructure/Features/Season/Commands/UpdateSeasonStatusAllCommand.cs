using BartonKeys.Functional;
using MediatR;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Series.Commands
{
    public class UpdateSeasonStatusAllCommand : IRequest<Result>
    {
        public int SeasonId { get; set; }
        public WorksStatus? WorksStatus { get; set; }
    }

    public class UpdateSeasonStatusAllCommandHandler : SimpleAbstractBaseHandler<UpdateSeasonStatusAllCommand>
    {
        public UpdateSeasonStatusAllCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<UpdateSeasonStatusAllCommand> validator, ILogger<UpdateSeasonStatusAllCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result> HandleRequest(UpdateSeasonStatusAllCommand request, CancellationToken cancellationToken)
        {
            var season = OscarContext.Seasons
                .Include(s => s.Episodes)
                .AsSplitQuery()
                .Single(s => s.Id == request.SeasonId);

            season.WorksStatus = request.WorksStatus;

            foreach (var episode in season.Episodes!)
            {
                episode.WorksStatus = request.WorksStatus;
                if (request.WorksStatus == WorksStatus.Uncontrolled)
                    episode.UncontrolledReason = season.UncontrolledReason;
            }

            await OscarContext.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}
