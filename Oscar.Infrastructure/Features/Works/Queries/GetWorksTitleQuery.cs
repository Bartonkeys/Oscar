using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Works.Queries
{
    public class GetWorksTitleQuery : BaseTableQuery, IRequest<Result<WorksTitleDto>>
    {
        public int Id { get; set; }
    }

    public class GetWorksTitleQueryHandler : AbstractBaseHandler<GetWorksTitleQuery, WorksTitleDto>
    {
        public GetWorksTitleQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetWorksTitleQuery> validator, ILogger<GetWorksTitleQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<WorksTitleDto>> HandleRequest(GetWorksTitleQuery request, CancellationToken cancellationToken)
        {
            Logger.LogInformation((int)WorksFeatureEvent.Get, CommandResult.SUCCESS);

            var worksTitle = await OscarContext
                .WorksTitles
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Works.Id == request.Id && (t.TitleType == TitleType.Main || t.TitleType == TitleType.Episode), cancellationToken: cancellationToken);

            return Result.Ok(Mapper.Map<WorksTitleDto>(worksTitle));
        }

    }
}
