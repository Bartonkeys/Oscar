using BartonKeys.Functional;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Series.Commands
{
    public class UpdateSeriesStatusAllCommand : IRequest<Result>
    {
        public int SeriesId { get; set; }
        public WorksStatus? WorksStatus { get; set; }
    }

    public class UpdateSeriesStatusAllCommandHandler : SimpleAbstractBaseHandler<UpdateSeriesStatusAllCommand>
    {
        public UpdateSeriesStatusAllCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<UpdateSeriesStatusAllCommand> validator, ILogger<UpdateSeriesStatusAllCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result> HandleRequest(UpdateSeriesStatusAllCommand request, CancellationToken cancellationToken)
        {
            var series = OscarContext.Series
                .Include(s => s.Seasons)!.ThenInclude(s => s.Episodes)
                .AsSplitQuery()
                .Single(s => s.Id == request.SeriesId);

            series.WorksStatus = request.WorksStatus;

            foreach (var season in series.Seasons!)
            {
                season.WorksStatus = request.WorksStatus;
                if(request.WorksStatus == WorksStatus.Uncontrolled)
                    season.UncontrolledReason = series.UncontrolledReason;
                foreach (var episode in season.Episodes!)
                {
                    episode.WorksStatus = request.WorksStatus;
                    if (request.WorksStatus == WorksStatus.Uncontrolled)
                        episode.UncontrolledReason = series.UncontrolledReason;
                }
            }


            var seriesEpisodes = OscarContext.Series
                .Include(s => s.Episodes)
                .AsSplitQuery()
                .Single(s => s.Id == request.SeriesId);

            seriesEpisodes.WorksStatus = request.WorksStatus;

            foreach (var episode in seriesEpisodes.Episodes!)
            {
                episode.WorksStatus = request.WorksStatus;
                if (request.WorksStatus == WorksStatus.Uncontrolled)
                    episode.UncontrolledReason = series.UncontrolledReason;
            }

            await OscarContext.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}
