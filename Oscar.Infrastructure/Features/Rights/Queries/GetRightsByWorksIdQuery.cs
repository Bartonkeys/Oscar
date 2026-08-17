using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Rights.Queries
{
    public class GetRightsByWorksIdQuery : IRequest<Result<IEnumerable<RightDto>>>
    {
        public int WorksId { get; set; }
    }

    public class GetRightsByWorksIdHandler : AbstractBaseHandler<GetRightsByWorksIdQuery, IEnumerable<RightDto>>
    {
        public GetRightsByWorksIdHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetRightsByWorksIdQuery> validator, ILogger<GetRightsByWorksIdQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEnumerable<RightDto>>> HandleRequest(GetRightsByWorksIdQuery request, CancellationToken cancellationToken)
        {
            var worksCatalogueIds = OscarContext
                .Works
                .Include(w => w.Catalogues)
                ?.Where(w => w.Id == request.WorksId).FirstOrDefault()
                ?.Catalogues?.Select(x => x.Id).ToList();

            var rights = await OscarContext
                .Rights
                .AsNoTracking()
                .Include(r => r.Type)
                .Include(cr => cr.ChannelRights).ThenInclude(t => t.Channel)
                .Include(cr => cr.ChannelRights).ThenInclude(t => t.CountryRights)
                .Include(lr => lr.LanguageRights).ThenInclude(l => l.Language)
                .Include(c => c.Countries)
                .Include(c => c.Work)
                .Include(c => c.Catalogue)
                .ThenInclude(c => c.Client)
                .AsSplitQuery()
                .Where(r => r.Work != null && r.Work.Id == request.WorksId && r.Catalogue != null && worksCatalogueIds != null && worksCatalogueIds.Contains(r.Catalogue.Id))
                .ToListAsync(cancellationToken: cancellationToken);

            var results = rights.Select(r => Mapper.Map<RightDto>(r));
            await Task.CompletedTask;
            return Result.Ok(results.AsEnumerable());
        }
    }

}
