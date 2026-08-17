using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Oscar.Core.Entities;
using Oscar.Core.Enums;

namespace Oscar.Infrastructure.Features.Common
{
    public static class EnumExtensions
    {
        public static ExpressionStarter<T> BuildSearchTypePredicate<T>(this SearchType searchType, ExpressionStarter<T> predicate, string title)
        where T : class, ITitle
        {
            switch (searchType)
            {
                case SearchType.FreeText:
                    predicate = predicate.And(t => EF.Functions.FreeText(t.Title, $"{title}"));
                    break;
                case SearchType.Contains:
                    predicate = predicate.And(t => EF.Functions.Contains(t.Title, $"{title}"));
                    break;
                case SearchType.ContainsExpression:
                    predicate = predicate.And(t => EF.Functions.Contains(t.Title, QueryHelpers.BuildContainsFullTextSearch(title))
                                                   || EF.Functions.Contains(t.Title, QueryHelpers.BuildContainsFullTextSearchPrefix(title)));
                    break;
                case SearchType.StartsWith:
                    predicate = predicate.And(t => EF.Functions.Like(t.Title, $"{title}%"));
                    break;
                case SearchType.Like:
                    predicate = predicate.And(t => EF.Functions.Like(t.Title, $"%{title}%"));
                    break;
                case SearchType.Equals:
                    predicate = predicate.And(t => t.Title == title);
                    break;
            }
            return predicate;
        }

        // This extension method is broken out so you can use a similar pattern with 
        // other MetaData elements in the future. This is your base method for each.
        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return attributes.Length > 0
              ? (T)attributes[0]
              : null;
        }

        // This method creates a specific call to the above method, requesting the
        // Description MetaData attribute.
        public static string ToName(this Enum value)
        {
            var attribute = value.GetAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

    }
}
