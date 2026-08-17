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

namespace Oscar.Infrastructure.Features.Director.Queries
{
    public class GetDirectorQuery : BaseTableQuery, IRequest<Result<IEntityTable<DirectorDto>>>
    {
        public int Id { get; set; }
    }

    public class GetDirectorQueryHandler : AbstractBaseHandler<GetDirectorQuery, IEntityTable<DirectorDto>>
    {
        public GetDirectorQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetDirectorQuery> validator, ILogger<GetDirectorQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEntityTable<DirectorDto>>> HandleRequest(GetDirectorQuery request, CancellationToken cancellationToken)
        {
            Logger.LogInformation((int)DirectorFeatureEvent.Get, CommandResult.SUCCESS);

            var directors = OscarContext.Directors;
            var total = directors.Count();

            return Result.Ok(EntityTable<DirectorDto>.Create(directors.Select(c => Mapper.Map<DirectorDto>(c))).WithTotal(total));
        }
        
    }
}
