using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oscar.Infrastructure.Features.Common
{
    public static class QueryHelpers
    {
        public static IOrderedQueryable<T> SortByProperty<T>(IQueryable<T> query, string propertyName, bool desc, string intermediatePropertyName = null)
        {
            var instance = Expression.Parameter(typeof(T));

            PropertyInfo property;
            MemberExpression getter;
            if (!string.IsNullOrWhiteSpace(intermediatePropertyName))
            {
                var intermediateProperty = typeof(T).GetProperty(intermediatePropertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                var inter = Expression.Property(instance, intermediateProperty);
                property = intermediateProperty.PropertyType.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                getter = Expression.Property(inter, property);
            }
            else
            {
                property = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                getter = Expression.Property(instance, property);
            }

            var func = Expression.Lambda<Func<T, object>>(Expression.Convert(getter, typeof(object)), instance);

            if (desc)
            {
                return query.OrderByDescending(func);
            }
            return query.OrderBy(func);
        }

        public static bool CanSort<T>(string propertyName, string intermediatePropertyName = null)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return false;
            }

            PropertyInfo property;
            if (!string.IsNullOrWhiteSpace(intermediatePropertyName))
            {
                var intermediateProperty = typeof(T).GetProperty(intermediatePropertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (intermediateProperty == null)
                {
                    return false;
                }

                property = intermediateProperty.PropertyType.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            }
            else
            {
                property = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            }

            return property != null;
        }

        public static string BuildStartsWithWildCard(string searchItem)
        {
            return !searchItem.EndsWith("%") ? searchItem + "%" : searchItem;
        }

        public static string BuildContainsWildCard(string searchItem)
        {
            searchItem = !searchItem.EndsWith("%") ? searchItem + "%" : searchItem;
            return searchItem.StartsWith("%") ? searchItem : "%" + searchItem;
        }

        public static string BuildContainsFullTextSearch(string searchItem)
        {
            if (string.IsNullOrWhiteSpace(searchItem))
            {
                return string.Empty;
            }

            return "\"" + searchItem.Trim().ToLower() + "\"";
        }

        public static string BuildContainsFullTextSearchPrefix(string searchItem)
        {
            if (string.IsNullOrWhiteSpace(searchItem))
            {
                return string.Empty;
            }

            return "\"" + searchItem.Trim().ToLower() + "*\"";
        }
    }
}
