namespace Oscar.Blazor.Library.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.Encodings.Web;

    /// <summary>Contains a LINQ query in a serializable format.</summary>
    public class Query
    {
        /// <summary>Gets or sets the filter.</summary>
        /// <value>The filter.</value>
        public string Filter { get; set; }

        /// <summary>
        /// Gets the filter expression as a collection of filter descriptors.
        /// </summary>
        /// <value>The filter parameters.</value>
        public IEnumerable<FilterDescriptor> Filters { get; set; }

        /// <summary>
        /// Gets the sort expression as a collection of sort descriptors.
        /// </summary>
        /// <value>The sorts.</value>
        public IEnumerable<SortDescriptor> Sorts { get; set; }

        /// <summary>Gets or sets the filter parameters.</summary>
        /// <value>The filter parameters.</value>
        public object[] FilterParameters { get; set; }

        /// <summary>Gets or sets the order by.</summary>
        /// <value>The order by.</value>
        public string OrderBy { get; set; }

        /// <summary>Gets or sets the expand.</summary>
        /// <value>The expand.</value>
        public string Expand { get; set; }

        /// <summary>Gets or sets the select.</summary>
        /// <value>The select.</value>
        public string Select { get; set; }

        /// <summary>Gets or sets the skip.</summary>
        /// <value>The skip.</value>
        public int? Skip { get; set; }

        /// <summary>Gets or sets the top.</summary>
        /// <value>The top.</value>
        public int? Top { get; set; }

        /// <summary>Converts the query to OData query format.</summary>
        /// <param name="url">The URL.</param>
        /// <returns>System.String.</returns>
        public string ToUrl(string url)
        {
            Dictionary<string, object> source = new Dictionary<string, object>();
            if (this.Skip.HasValue)
                source.Add("$skip", (object)this.Skip.Value);
            if (this.Top.HasValue)
                source.Add("$top", (object)this.Top.Value);
            if (!string.IsNullOrEmpty(this.OrderBy))
                source.Add("$orderBy", (object)this.OrderBy);
            if (!string.IsNullOrEmpty(this.Filter))
                source.Add("$filter", (object)UrlEncoder.Default.Encode(this.Filter));
            if (!string.IsNullOrEmpty(this.Expand))
                source.Add("$expand", (object)this.Expand);
            if (!string.IsNullOrEmpty(this.Select))
                source.Add("$select", (object)this.Select);
            return string.Format("{0}{1}", (object)url, source.Any<KeyValuePair<string, object>>() ? (object)("?" + string.Join("&", source.Select<KeyValuePair<string, object>, string>((Func<KeyValuePair<string, object>, string>)(a =>
            {
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
                interpolatedStringHandler.AppendFormatted(a.Key);
                interpolatedStringHandler.AppendLiteral("=");
                interpolatedStringHandler.AppendFormatted<object>(a.Value);
                return interpolatedStringHandler.ToStringAndClear();
            })))) : (object)"");
        }

        public override string ToString()
        {
            return ToUrl(string.Empty);
        }
    }

    public class FilterDescriptor
    {
        public string Property { get; set; }
        public object FilterValue { get; set; }
        public FilterOperator FilterOperator { get; set; }
        public object SecondFilterValue { get; set; }
        public FilterOperator SecondFilterOperator { get; set; }
        public LogicalFilterOperator LogicalFilterOperator { get; set; }
    }

    public class SortDescriptor
    {
        public string Property { get; set; }
        public SortOrder? SortOrder { get; set; }
    }

    public enum SortOrder
    {
        Ascending,
        Descending,
    }

    public enum FilterOperator
    {
        Equals,
        NotEquals,
        LessThan,
        LessThanOrEquals,
        GreaterThan,
        GreaterThanOrEquals,
        Contains,
        StartsWith,
        EndsWith,
        DoesNotContain,
        In,
        NotIn,
        IsNull,
        IsEmpty,
        IsNotNull,
        IsNotEmpty,
        Custom,
    }

    public enum LogicalFilterOperator
    {
        And,
        Or,
    }

    public static class QueryExtensions
    {
        public static void AddOrAppendFilter(this Query query, LogicalFilterOperator filterOp, string condition, object filterParam = null)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            string filterOpString = LogicalFilterOperator.And == filterOp ? "&&" : "||"; //filterOp.ToString().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(query.Filter))
                query.Filter = condition;
            else 
                query.Filter += $" {filterOpString} {condition}";

            if (filterParam != null)
            {
                int paramCount = (query.FilterParameters != null) ? query.FilterParameters.Length : 0;
                object[] parameters = new object[paramCount+1];
                if (query.FilterParameters != null) { query.FilterParameters.CopyTo(parameters, 0); }
                parameters[paramCount] = filterParam;
                query.FilterParameters = parameters;
            }
        }
    }
}
