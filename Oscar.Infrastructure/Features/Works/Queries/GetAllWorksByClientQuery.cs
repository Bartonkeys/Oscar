using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Series.Queries
{
    public class GetWorksByClientQuery : BaseTableQuery, IRequest<Result<IQueryable<LightWeightWorksDto>>>
    {
        public int ClientID { get; set; }
    }

    public class GetWorksByClientQueryHandler : AbstractBaseHandler<GetWorksByClientQuery, IQueryable<LightWeightWorksDto>>
    {
        public GetWorksByClientQueryHandler(OscarContext oscarContext, 
            IMapper mapper, 
            IValidator<GetWorksByClientQuery> validator, 
            ILogger<GetWorksByClientQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IQueryable<LightWeightWorksDto>>> HandleRequest(GetWorksByClientQuery request, CancellationToken cancellationToken)
        {
            var ids = OscarContext
                .Clients
                .Include( c=> c.Works)
                .Where(c => c.Id == request.ClientID)
                .FirstOrDefault()
                .Works
                .Select(w => w.Id);

            var works = OscarContext.Works
                .Where ( w=> ids.Contains(w.Id))
                .Select(s => new LightWeightWorksDto
                {
                    Id = s.Id,
                    Title = s.Titles.First().Title,
                    ReverseTitle = s.Titles.First().ReverseTitle
                });

            return Result.Ok(works);
        }
    }
}
