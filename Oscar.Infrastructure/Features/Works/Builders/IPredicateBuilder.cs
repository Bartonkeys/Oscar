using LinqKit;
using Microsoft.EntityFrameworkCore;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Works.Queries;

namespace Oscar.Infrastructure.Features.Works.Builders;

public interface IPredicateBuilder
{
    ExpressionStarter<Core.Entities.Works> Build(SearchWorksQuery request,
        ExpressionStarter<Core.Entities.Works> predicate);
}

public class ActorPredicateBuilder : IPredicateBuilder
{
    public ExpressionStarter<Core.Entities.Works> Build(SearchWorksQuery request, ExpressionStarter<Core.Entities.Works> predicate)
    {
        if (!string.IsNullOrEmpty(request.ActorFirstName) && !string.IsNullOrEmpty(request.ActorLastName))
            predicate = predicate.And(c => c.Actors.Any(a => EF.Functions.Like(a.FirstName, "%" + request.ActorFirstName + "%")
                                                             && EF.Functions.Like(a.LastName, "%" + request.ActorLastName + "%")));
        else if (!string.IsNullOrEmpty(request.ActorFirstName))
            predicate = predicate.And(c => c.Actors.Any(t => EF.Functions.Like(t.FirstName, "%" + request.ActorFirstName + "%")));
        else if (!string.IsNullOrEmpty(request.ActorLastName))
            predicate = predicate.And(c => c.Actors.Any(t => EF.Functions.Like(t.LastName, "%" + request.ActorLastName + "%")));

        return predicate;
    }
}

public class DirectorPredicateBuilder : IPredicateBuilder
{
    public ExpressionStarter<Core.Entities.Works> Build(SearchWorksQuery request, ExpressionStarter<Core.Entities.Works> predicate)
    {
        if (!string.IsNullOrEmpty(request.DirectorFirstName) && !string.IsNullOrEmpty(request.DirectorLastName))
            predicate = predicate.And(c => c.Directors.Any(a => EF.Functions.Like(a.FirstName, "%" + request.DirectorFirstName + "%")
                                                             && EF.Functions.Like(a.LastName, "%" + request.DirectorLastName + "%")));
        else if (!string.IsNullOrEmpty(request.DirectorFirstName))
            predicate = predicate.And(c => c.Directors.Any(t => EF.Functions.Like(t.FirstName, "%" + request.DirectorFirstName + "%")));
        else if (!string.IsNullOrEmpty(request.DirectorLastName))
            predicate = predicate.And(c => c.Directors.Any(t => EF.Functions.Like(t.LastName, "%" + request.DirectorLastName + "%")));

        return predicate;
    }
}

public class ProducerPredicateBuilder : IPredicateBuilder
{
    public ExpressionStarter<Core.Entities.Works> Build(SearchWorksQuery request, ExpressionStarter<Core.Entities.Works> predicate)
    {
        if (!string.IsNullOrEmpty(request.ProducerFirstName) && !string.IsNullOrEmpty(request.ProducerLastName))
            predicate = predicate.And(c => c.Producers.Any(a => EF.Functions.Like(a.FirstName, "%" + request.ProducerFirstName + "%")
                                                             && EF.Functions.Like(a.LastName, "%" + request.ProducerLastName + "%")));
        else if (!string.IsNullOrEmpty(request.ProducerFirstName))
            predicate = predicate.And(c => c.Producers.Any(t => EF.Functions.Like(t.FirstName, "%" + request.ProducerFirstName + "%")));
        else if (!string.IsNullOrEmpty(request.ProducerLastName))
            predicate = predicate.And(c => c.Producers.Any(t => EF.Functions.Like(t.LastName, "%" + request.ProducerLastName + "%")));

        return predicate;
    }
}

public class ScreenWriterPredicateBuilder : IPredicateBuilder
{
    public ExpressionStarter<Core.Entities.Works> Build(SearchWorksQuery request, ExpressionStarter<Core.Entities.Works> predicate)
    {
        if (!string.IsNullOrEmpty(request.ScreenWriterFirstName) && !string.IsNullOrEmpty(request.ActorLastName))
            predicate = predicate.And(c => c.ScreenWriters.Any(a => EF.Functions.Like(a.FirstName, "%" + request.ScreenWriterFirstName + "%")
                                                             && EF.Functions.Like(a.LastName, "%" + request.ScreenWriterLastName + "%")));
        else if (!string.IsNullOrEmpty(request.ScreenWriterFirstName))
            predicate = predicate.And(c => c.ScreenWriters.Any(t => EF.Functions.Like(t.FirstName, "%" + request.ScreenWriterFirstName + "%")));
        else if (!string.IsNullOrEmpty(request.ScreenWriterLastName))
            predicate = predicate.And(c => c.ScreenWriters.Any(t => EF.Functions.Like(t.LastName, "%" + request.ScreenWriterLastName + "%")));

        return predicate;
    }
}

public class ProductionYearPredicateBuilder : IPredicateBuilder
{
    public ExpressionStarter<Core.Entities.Works> Build(SearchWorksQuery request, ExpressionStarter<Core.Entities.Works> predicate)
    {
        if (request.ProductionYear != null)
            predicate = predicate.And(c => c.ProductionYear == request.ProductionYear);

        return predicate;
    }
}

public class BroadcastYearPredicateBuilder : IPredicateBuilder
{
    public ExpressionStarter<Core.Entities.Works> Build(SearchWorksQuery request, ExpressionStarter<Core.Entities.Works> predicate)
    {
        if (request.FirstBroadcastYear != null)
            predicate = predicate.And(c => c.FirstBroadcastYear == request.FirstBroadcastYear);

        return predicate;
    }
}

public class RightsCountryPredicateBuilder : IPredicateBuilder
{
    public ExpressionStarter<Core.Entities.Works> Build(SearchWorksQuery request, ExpressionStarter<Core.Entities.Works> predicate)
    {
        if (request.RightsCountryID != null && request.RightsCountryID > 0)
            predicate = predicate.And(c => c.Rights != null && c.Rights.SelectMany(r => r.Countries).Any(c => c.Id == request.RightsCountryID));

        return predicate;
    }
}

public class WorksTypePredicateBuilder : IPredicateBuilder
{
    public ExpressionStarter<Core.Entities.Works> Build(SearchWorksQuery request, ExpressionStarter<Core.Entities.Works> predicate)
    {
        if (request.WorksTypeId != null)
            predicate = predicate.And(w => w.WorksTypeId == request.WorksTypeId);

        return predicate;
    }
}

public class HasNoRightsPredicateBuilder : IPredicateBuilder
{
    public ExpressionStarter<Core.Entities.Works> Build(SearchWorksQuery request, ExpressionStarter<Core.Entities.Works> predicate)
    {
        if (request.HasNoRights != null)
            predicate.And(r =>
                request.HasNoRights!.Value ? r.Rights == null || !r.Rights.Any() : r.Rights != null && r.Rights.Any());

        return predicate;
    }
}

public class DateCreatedFromPredicateBuilder : IPredicateBuilder
{
    public ExpressionStarter<Core.Entities.Works> Build(SearchWorksQuery request, ExpressionStarter<Core.Entities.Works> predicate)
    {
        if (request.DateCreatedFrom != null)
            predicate.And(w => w.CreationDate >= request.DateCreatedFrom);

        return predicate;
    }
}

public class DateCreatedToPredicateBuilder : IPredicateBuilder
{
    public ExpressionStarter<Core.Entities.Works> Build(SearchWorksQuery request, ExpressionStarter<Core.Entities.Works> predicate)
    {
        if (request.DateCreatedTo != null)
            predicate.And(w => w.CreationDate <= request.DateCreatedTo);

        return predicate;
    }
}

public class AgicoaRefPredicateBuilder : IPredicateBuilder
{
    public ExpressionStarter<Core.Entities.Works> Build(SearchWorksQuery request, ExpressionStarter<Core.Entities.Works> predicate)
    {
        if (!string.IsNullOrEmpty(request.SearchStringAgicoaRef))
            //predicate.And(w => w.AgicoaWorksReference.ToLower().Contains(request.SearchStringAgicoaRef.ToLower()));
            predicate.And(w => w.AgicoaWorksReference != null && EF.Functions.Like(w.AgicoaWorksReference, $"%{request.SearchStringAgicoaRef}%"));

        return predicate;
    }
}

public class CompactRefPredicateBuilder : IPredicateBuilder
{
    public ExpressionStarter<Core.Entities.Works> Build(SearchWorksQuery request, ExpressionStarter<Core.Entities.Works> predicate)
    {
        if (!string.IsNullOrEmpty(request.SearchStringCompactRef))
            predicate.And(w => w.CompactRef != null && EF.Functions.Like(w.CompactRef, $"%{request.SearchStringCompactRef}%"));

        return predicate;
    }
}

public class AS400PredicateBuilder : IPredicateBuilder
{
    public ExpressionStarter<Core.Entities.Works> Build(SearchWorksQuery request, ExpressionStarter<Core.Entities.Works> predicate)
    {
        if (!string.IsNullOrEmpty(request.SearchStringAS400))
            //predicate.And(w => w.AS400RefNo.ToLower().Contains(request.SearchStringAS400.ToLower()));
            predicate.And(w => w.AS400RefNo != null && EF.Functions.Like(w.AS400RefNo, $"%{request.SearchStringAS400}%"));

        return predicate;
    }
}

public class ProductionCompanyPredicateBuilder : IPredicateBuilder
{
    public ExpressionStarter<Core.Entities.Works> Build(SearchWorksQuery request, ExpressionStarter<Core.Entities.Works> predicate)
    {
        if (request.ProductionCompanyID != null && request.ProductionCompanyID > 0)
            predicate.And(c => c.Companies.Any(c => c.Id == request.ProductionCompanyID));

        return predicate;
    }
}

public class WorksTitlePredicateBuilder : IPredicateBuilder
{
    public ExpressionStarter<Core.Entities.Works> Build(SearchWorksQuery request, ExpressionStarter<Core.Entities.Works> predicate)
    {
        if (!string.IsNullOrEmpty(request.Title))
        {
            switch (request.SearchType)
            {
                case SearchType.FreeText:
                    predicate = predicate.And(c => c.Titles.Any(t => (request.IncludeAlternateTitles ? 1 == 1 : (t.TitleType == TitleType.Main || t.TitleType == TitleType.Episode)) && EF.Functions.FreeText(t.Title, $"{request.Title}")));
                    break;
                case SearchType.Contains:
                    predicate = predicate.And(c => c.Titles.Any(t => (request.IncludeAlternateTitles ? 1 == 1 : (t.TitleType == TitleType.Main || t.TitleType == TitleType.Episode)) && EF.Functions.Contains(t.Title, $"{request.Title}")));
                    break;
                case SearchType.ContainsExpression:
                    predicate = predicate.And(c => c.Titles.Any(t => (request.IncludeAlternateTitles ? 1 == 1 : (t.TitleType == TitleType.Main || t.TitleType == TitleType.Episode)) &&
                                                                     (EF.Functions.Contains(t.Title, QueryHelpers.BuildContainsFullTextSearch(request.Title)) || EF.Functions.Contains(t.Title, QueryHelpers.BuildContainsFullTextSearchPrefix(request.Title)))));
                    break;
                case SearchType.StartsWith:
                    predicate = predicate.And(c => c.Titles.Any(t => (request.IncludeAlternateTitles ? 1 == 1 : (t.TitleType == TitleType.Main || t.TitleType == TitleType.Episode)) &&
                                                                     EF.Functions.Like(t.Title, $"{request.Title}%")));
                    break;
                case SearchType.Like:
                    predicate = predicate.And(c => c.Titles.Any(t => (request.IncludeAlternateTitles ? 1 == 1 : (t.TitleType == TitleType.Main || t.TitleType == TitleType.Episode)) &&
                                                                     EF.Functions.Like(t.Title, $"%{request.Title}%")));
                    break;
                case SearchType.Equals:
                    predicate = predicate.And(c => c.Titles.Any(t => (request.IncludeAlternateTitles ? 1 == 1 : (t.TitleType == TitleType.Main || t.TitleType == TitleType.Episode)) && t.Title.Equals(request.Title)));
                    break;
            }
        }

        return predicate;
    }
}
