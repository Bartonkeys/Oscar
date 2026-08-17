using System.Linq.Expressions;
using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Rights.Queries
{
    public class GetRightsByClientIdQuery: IRequest<Result<IEnumerable<RightDto>>>
    {
        public int ClientId { get; set; }
        public int? CatalogueId { get; set; }
    }

    public class GetRightsByClientIdHandler : AbstractBaseHandler<GetRightsByClientIdQuery, IEnumerable<RightDto>>
    {
        public GetRightsByClientIdHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetRightsByClientIdQuery> validator, ILogger<GetRightsByClientIdQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEnumerable<RightDto>>> HandleRequest(GetRightsByClientIdQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Right, bool>> predicate = request.CatalogueId == null
                ? r => r.Client != null && r.Client.Id == request.ClientId && r.Work == null
                : r => r.Client != null && r.Client.Id == request.ClientId && r.Work == null && r.Catalogue.Id == request.CatalogueId;

            var rights = await OscarContext
                .Rights
                .AsNoTracking()
                .Include(r => r.Type)
                .Include(cr => cr.ChannelRights).ThenInclude(t => t.Channel)
                .Include(cr => cr.ChannelRights).ThenInclude(t => t.CountryRights)
                .Include(lr => lr.LanguageRights).ThenInclude(l => l.Language)
                .Include(c => c.Countries)
                .Include(c => c.Catalogue)
                .AsSplitQuery()
                .Where(predicate)
                .ToListAsync(cancellationToken: cancellationToken);
            
            var results = rights.Select(r => Mapper.Map<RightDto>(r));

            return Result.Ok(results.AsEnumerable());
        }
    }

}
