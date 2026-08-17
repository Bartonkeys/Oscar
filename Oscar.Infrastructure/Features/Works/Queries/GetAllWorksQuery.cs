using System.Linq.Expressions;
using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Series.Queries;

public class GetAllWorksQuery : BaseTableQuery, IRequest<Result<IQueryable<LightWeightWorksDto>>>
{
    public Discriminator Discriminator { get; set; }
    public string Title { get; set; }
    public int? ClientId { get; set; }
    public int? CatalogueId { get; set; }
}

public class GetAllWorksQueryHandler : AbstractBaseHandler<GetAllWorksQuery, IQueryable<LightWeightWorksDto>>
{
    public GetAllWorksQueryHandler(OscarContext oscarContext,
        IMapper mapper,
        IValidator<GetAllWorksQuery> validator,
        ILogger<GetAllWorksQuery> logger) : base(oscarContext, mapper, validator, logger)
    {
    }

    protected override async Task<Result<IQueryable<LightWeightWorksDto>>> HandleRequest(GetAllWorksQuery request,
        CancellationToken cancellationToken)
    {
        Expression<Func<Core.Entities.Works, bool>> predicate = request.CatalogueId != null
            ? w => request.Discriminator == Discriminator.All ||
                   w.Discriminator == request.Discriminator.ToString() && 
                   w.Clients.Any(c => c.Id == request.ClientId) &&
                   w.Catalogues.Any(c => c.Id == request.CatalogueId) : 
            w => request.Discriminator == Discriminator.All ||
                 w.Discriminator == request.Discriminator.ToString() && 
                 w.Clients.Any(c => c.Id == request.ClientId);

        var works = OscarContext
            .Works
            .Where(predicate)
            .Select(s => new LightWeightWorksDto
            {
                Id = s.Id,
                Title = s.Titles.First().Title,
                ReverseTitle = s.Titles.First().ReverseTitle
            });

        return Result.Ok(works);
    }
}