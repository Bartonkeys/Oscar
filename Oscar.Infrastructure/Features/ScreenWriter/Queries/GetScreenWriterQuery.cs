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

namespace Oscar.Infrastructure.Features.ScreenWriter.Queries
{
    public class GetScreenWriterQuery : BaseTableQuery, IRequest<Result<IEntityTable<ScreenWriterDto>>>
    {
        public int Id { get; set; }
    }

    public class GetScreenWriterQueryHandler : AbstractBaseHandler<GetScreenWriterQuery, IEntityTable<ScreenWriterDto>>
    {
        public GetScreenWriterQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetScreenWriterQuery> validator, ILogger<GetScreenWriterQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEntityTable<ScreenWriterDto>>> HandleRequest(GetScreenWriterQuery request, CancellationToken cancellationToken)
        {
            Logger.LogInformation((int)ScreenWriterFeatureEvent.Get, CommandResult.SUCCESS);

            var screenWriters = OscarContext.ScreenWriters;
            var total = screenWriters.Count();

            return Result.Ok(EntityTable<ScreenWriterDto>.Create(screenWriters.Select(c => Mapper.Map<ScreenWriterDto>(c))).WithTotal(total));
        }
        
    }
}
