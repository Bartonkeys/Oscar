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

namespace Oscar.Infrastructure.Features.Actor.Queries
{
    public class GetActorQuery : BaseTableQuery, IRequest<Result<IEntityTable<ActorDto>>>
    {
        public int Id { get; set; }
    }

    public class GetActorQueryHandler : AbstractBaseHandler<GetActorQuery, IEntityTable<ActorDto>>
    {
        public GetActorQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetActorQuery> validator, ILogger<GetActorQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEntityTable<ActorDto>>> HandleRequest(GetActorQuery request, CancellationToken cancellationToken)
        {
            Logger.LogInformation((int)ActorFeatureEvent.Get, CommandResult.SUCCESS);

            var actors = await OscarContext.Actors.ToListAsync(cancellationToken);
            var total = actors.Count();

            return Result.Ok(EntityTable<ActorDto>.Create(actors.Select(c => Mapper.Map<ActorDto>(c))).WithTotal(total));
        }
        
    }
}
