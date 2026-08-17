using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Oscar.Core.Entities;

namespace Oscar.Infrastructure.Features.Works.Queries
{
    public class GetWorksTitleAutoCompleteQuery : IRequest<Result<IEnumerable<WorksTitleResponseDto>>>
    {
        public string? Title { get; set; }
        public int MaxCount { get; set; } = 10;
        public SearchType SearchType { get; set; } = SearchType.StartsWith;
    }

    public class GetWorksTitleAutoCompleteQueryHandler : AbstractBaseHandler<GetWorksTitleAutoCompleteQuery, IEnumerable<WorksTitleResponseDto>>
    {
        public GetWorksTitleAutoCompleteQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetWorksTitleAutoCompleteQuery> validator, ILogger<GetWorksTitleAutoCompleteQuery> logger)
            : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEnumerable<WorksTitleResponseDto>>> HandleRequest(GetWorksTitleAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            var titles = await OscarContext
                .WorksTitles
                .AsExpandable()
                .AsNoTracking()
                .Where(BuildPredicate(request))
                .Take(request.MaxCount * 5)
                .Select(t => new WorksTitleResponseDto
                {
                    WorksId = t.Works.Id,
                    Title = $"{t.Title}"
                }).ToListAsync(cancellationToken: cancellationToken);

            var distinctTitles = titles.DistinctBy(t => t.Title).Take(request.MaxCount);

            return Result.Ok(distinctTitles.AsEnumerable());
        }

        public Expression<Func<WorksTitle, bool>> BuildPredicate(GetWorksTitleAutoCompleteQuery request)
        {
            var predicate = PredicateBuilder.New<WorksTitle>(true);

            predicate = request.SearchType.BuildSearchTypePredicate(predicate, request.Title);


            predicate = predicate.And(c => c.Works.WorksStatus != Core.Enums.WorksStatus.Uncontrolled);


            return predicate;
        }
    }
}
