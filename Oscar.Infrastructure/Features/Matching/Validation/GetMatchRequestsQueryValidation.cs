using FluentValidation;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Matching.Queries;

namespace Oscar.Infrastructure.Features.Matching.Validation
{
    public class GetMatchRequestsQueryValidation: AbstractValidator<GetMatchRequestsQuery>
    {
        public GetMatchRequestsQueryValidation()
        {
            var sortDirections = new List<string>() { "descending", "ascending" };
            RuleFor(r => r.SortDirection)
              .Must(sd => string.IsNullOrEmpty(sd) || sortDirections.Contains(sd.ToLower()))
              .WithMessage("Sort direction must be one of: " + String.Join(",", sortDirections));

            var sortColumns = new List<string>() { "id", "status", "reference", "requestedby", "creationdate", "clientid" };
            RuleFor(r => r.SortColumn)
              .Must(sc => string.IsNullOrEmpty(sc) || sortColumns.Contains(sc.ToLower()))
              .WithMessage("{PropertyValue} Sort column must be one of: " + String.Join(",", sortColumns));

            RuleFor(r => r.Start)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Start must greater than or equal to 0");

            RuleForEach(r => r.SearchObjects).SetValidator(new SearchObjectValidator());
        }
    }

    public class SearchObjectValidator : AbstractValidator<SearchObject>
    {
        public SearchObjectValidator()
        {
            var searchColumnTypes = new List<string>() { "number", "string" };
            RuleFor(r => r.SearchColumnType)
              .Must(sct => string.IsNullOrEmpty(sct) || searchColumnTypes.Contains(sct.ToLower()))
              .WithMessage("Search column type must be one of: " + String.Join(",", searchColumnTypes));

            var searchColumns = new List<string>() { "status", "reference", "requestedby", "clientid" };
            RuleFor(r => r.SearchColumn)
               .Must(sc => string.IsNullOrEmpty(sc) || searchColumns.Contains(sc.ToLower()))
              .WithMessage("Search column must be one of: " + String.Join(",", searchColumns));

            RuleFor(r => r.SearchText)
                .Must(st => int.TryParse(st, out int g) == true)
                .When(r => r.SearchColumnType == "number")
                .WithMessage("Search text must be integer if column type is number");
        }
    }
}
