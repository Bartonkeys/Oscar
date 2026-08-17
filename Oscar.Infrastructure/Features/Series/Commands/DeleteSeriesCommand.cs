using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Series.Commands
{
    public class DeleteSeriesCommand : IRequest<Result<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteSeriesCommandHandler : AbstractBaseHandler<DeleteSeriesCommand, string>
    {
        public DeleteSeriesCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<DeleteSeriesCommand> validator, ILogger<DeleteSeriesCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<string>> HandleRequest(DeleteSeriesCommand request, CancellationToken cancellationToken)
        {
            var series = OscarContext.Series
                .Include(e => e.Companies)
                .Include(e => e.Titles)
                .Include(e => e.Conflicts)
                .Include(e => e.WorksType)
                .Include(e => e.AlternativeTitles)
                .Include(e => e.Episodes)
                .Include(s => s.Seasons)
                .Include("Seasons.Companies")
                .Include("Seasons.Titles")
                .Include("Seasons.Conflicts")
                .Include("Seasons.WorksSubTypes")
                .Include("Seasons.AlternativeTitles")
                .Include("Seasons.Episodes")
                .Include("Seasons.Episodes.Companies")
                .Include("Seasons.Episodes.Titles")
                .Include("Seasons.Episodes.Conflicts")
                .Include("Seasons.Episodes.WorksSubTypes")
                .Include("Seasons.Episodes.AlternativeTitles")
                .FirstOrDefault(s => s.Id == request.Id);

            if (series == null)
            {
                Logger.LogInformation((int)SeriesFeatureEvent.DeleteNotFound, $"Not found {request.Id}");
                return Result.Fail<string>(CommandResult.NOTFOUND);
            }
            WorksHelper.RemoveSeries(series, OscarContext);

            await OscarContext.SaveChangesAsync(cancellationToken);

            Logger.LogInformation((int)SeriesFeatureEvent.Delete, CommandResult.SUCCESS);
            return Result.Ok(CommandResult.SUCCESS);
        }

    }
}
