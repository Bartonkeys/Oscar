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

namespace Oscar.Infrastructure.Features.SocietyReferences.Queries
{
    public class GetSocietyReferencesByWorksIdQuery : BaseTableQuery, IRequest<Result<List<SocietyReferenceDto>>>
    {
        public int WorksId { get; set; }
    }

    public class GetSocietyReferencesByWorksIdQueryHandler : AbstractBaseHandler<GetSocietyReferencesByWorksIdQuery, List<SocietyReferenceDto>>
    {
        public GetSocietyReferencesByWorksIdQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetSocietyReferencesByWorksIdQuery> validator, ILogger<GetSocietyReferencesByWorksIdQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<List<SocietyReferenceDto>>> HandleRequest(GetSocietyReferencesByWorksIdQuery request, CancellationToken cancellationToken)
        {
            OscarContext.ChangeTracker.LazyLoadingEnabled = false;
            var SocietyReferences = await OscarContext
                .SocietyReferences
                .AsNoTracking()
                .Include(s => s.Society)
                .Where(c => c.Works.Id == request.WorksId)
                .ToListAsync();

            if (SocietyReferences == null)
                return Result.Fail<List<SocietyReferenceDto>>("Not found");

            Logger.LogInformation((int)SocietyReferenceFeatureEvent.Get, CommandResult.SUCCESS);
            return Result.Ok(Mapper.Map<List<SocietyReferenceDto>>(SocietyReferences));
        }
    }
}
