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
    public class GetWorksByIdQuery: BaseTableQuery, IRequest<Result<WorksDto>>
    {
        public int Id { get; set; }
    }

    public class WorksByIdHandler : AbstractBaseHandler<GetWorksByIdQuery, WorksDto>
    {
        public WorksByIdHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetWorksByIdQuery> validator, ILogger<GetWorksByIdQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<WorksDto>> HandleRequest(GetWorksByIdQuery request, CancellationToken cancellationToken)
        {
            Logger.LogInformation((int)WorksFeatureEvent.Get, $"GET By Id {request.Id}");
            var works = await OscarContext.Works
                .Include(w => w.Titles)
                .Include(w => w.Countries)
                .Include(w => w.Conflicts)
                .Include(w => w.Clients)
                .Include(w => w.Catalogues)
                .Include(w => w.Directors)
                .Include(w => w.Producers)
                .Include(w => w.Actors)
                .Include(w => w.ScreenWriters)
                .Include(i => i.WorksSubType)
                .Include(i => i.WorksType)
                .SingleOrDefaultAsync(w => w.Id == request.Id, cancellationToken: cancellationToken);

            var worksDto = Mapper.Map<WorksDto>(works);
            Logger.LogInformation((int)WorksFeatureEvent.Get, CommandResult.SUCCESS);

            return Result.Ok(worksDto);
        }
    }
}
