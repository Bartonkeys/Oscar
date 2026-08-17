using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Catalogue.Queries;

public class GetCatalogueRightsQuery : BaseTableQuery, IRequest<Result<IEnumerable<RightDto>>>
{
    public int Id { get; set; }
}

public class GetCatalogueRightsHandler : AbstractBaseHandler<GetCatalogueRightsQuery, IEnumerable<RightDto>>
{
    public GetCatalogueRightsHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetCatalogueRightsQuery> validator, ILogger<GetCatalogueRightsQuery> logger) : base(oscarContext, mapper, validator, logger)
    {
    }

    protected override async Task<Result<IEnumerable<RightDto>>> HandleRequest(GetCatalogueRightsQuery request, CancellationToken cancellationToken)
    {
        var catalogueRights = await OscarContext.Catalogues
            .AsNoTracking()
            .Where(c => c.Id == request.Id)
            .SelectMany(c => c.Rights)
            .Include(t => t.Type)
            .Include(t => t.Work)
            .Include(c => c.Countries)
            .Include(c => c.LanguageRights).ThenInclude(l => l.Language)
            .Include(c => c.ChannelRights).ThenInclude(l => l.Channel)
            .Where(r => r.Work == null)
            .Select(r => Mapper.Map<RightDto>(r))
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        return catalogueRights.Any()
            ? Result.Ok(catalogueRights.AsEnumerable())
            : Result.Fail<IEnumerable<RightDto>>("No rights");
    }
}