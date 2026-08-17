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

namespace Oscar.Infrastructure.Features.StandAlone.Queries
{
    public class StandAloneSearchForDuplicate : BaseTableQuery, IRequest<Result<Boolean>>
    {
        public String Title { get; set; }
        public int? ProductionYear { get; set; }
        public int? GenreId { get; set; }
        public int? DurationMinutes { get; set; }
        public ICollection<int> CountryIds { get; set; }
    }

    public class StandAloneDuplicateSearchHandler : AbstractBaseHandler<StandAloneSearchForDuplicate, Boolean>
    {
        public StandAloneDuplicateSearchHandler(OscarContext oscarContext, IMapper mapper, IValidator<StandAloneSearchForDuplicate> validator, ILogger<StandAloneSearchForDuplicate> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<Boolean>> HandleRequest(StandAloneSearchForDuplicate request, CancellationToken cancellationToken)
        {

            bool series = OscarContext.StandAlones
            .Any(s =>
                    s.Titles.Any(t => t.TitleType == TitleType.Main && t.Title == request.Title) &&
                    s.ProductionYear == request.ProductionYear &&
                    s.GenreId == request.GenreId &&
                    s.DurationMinutes == request.DurationMinutes &&
                    s.Countries.Any(c => request.CountryIds.Any(cc => cc == c.Id))
                );

            Logger.LogInformation((int)StandAloneFeatureEvent.Get, CommandResult.SUCCESS);
            return Result.Ok(series);
        }
    }
}
