using System;
using Oscar.Data.Context;
using Oscar.Core.DTOs;
using System.Text;
using Newtonsoft.Json;
using Oscar.Core.Entities;
using Oscar.Infrastructure.Features.Common;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using Oscar.Core.Enums;
using System.Reflection;

namespace Oscar.Infrastructure.Features.Report.Services
{
    public static class ReportHelperService
    {
        private const string ENUM_ASSEMBLY_REF = "Oscar.Core.Enums.";

        internal static List<string>? BuildIncludesFromReportFields(ReportDto reportDto)
        {
            List<string> includes = new List<string>();
            var allFields = reportDto.ReportFields;

            if (allFields != null)
            {
                var distinctFields = allFields.GroupBy(x => x.BaseEntityName).Select(x => x.First()).ToArray();
                foreach (var field in distinctFields)
                {
                    if (field.BaseEntityName != reportDto.BaseEntityName && field.BaseEntityName != null)
                    {
                        includes.Add(field.BaseEntityName);
                    }
                }
            }

            return includes;
        }

        internal static List<SelectObject>? BuildSelectObjectsFromReportFields(ReportDto reportDto)
        {
            var selectObjects = new List<SelectObject>();
            var allFields = reportDto.ReportFields;
            if (allFields != null)
            {
                foreach (var field in allFields)
                {
                    selectObjects.Add(new SelectObject(field.BaseEntityName, field.ReportFieldName));
                }
            }
            return selectObjects;
        }

        internal static string? BuildJoinClauseFromReportFields(ReportDto reportDto, List<SearchObject> searchObjects, OscarContext oscarContext)
        {
            string joinClause = "";

            List<string> includes = new List<string>();
            List<ReportFieldDto> allFields = reportDto.ReportFields.ToList();

            List<ReportFieldDto> searchObjectsAsReportFields = searchObjects.Select(a => new ReportFieldDto()
            {
                BaseEntityName = a.SearchEntity,
                ReportFieldName = a.SearchColumn
            }).ToList();
            allFields.AddRange(searchObjectsAsReportFields);

            var allBaseEntities = oscarContext.ReportentityJoins.Select(x => x.BaseEntityName).Distinct().ToList();

            if (allFields != null)
            {
                var distinctFields = allFields.GroupBy(x => x.BaseEntityName).Select(x => x.First()).ToArray();
                foreach (var field in distinctFields)
                {
                    if (field.BaseEntityName != reportDto.BaseEntityName && field.BaseEntityName != null)
                    {
                        if (allBaseEntities.Contains(field.BaseEntityName))
                        {
                            includes.Insert(0, field.BaseEntityName);
                        }
                        else
                        {
                            includes.Add(field.BaseEntityName);
                        }
                    }
                }

                //Check that all required entity joins are included
                //NB: any top level joins are inserted at the top of the "WHERE" clause
                var reportEntityJoins = oscarContext.ReportentityJoins.Where
                        (x => includes.Contains(x.JoinEntityName)).ToList();
                foreach (var joins in reportEntityJoins)
                {
                    if (!includes.Contains(joins.BaseEntityName) && joins.BaseEntityName != reportDto.BaseEntityName)
                    {
                        includes.Insert(0, joins.BaseEntityName);
                    }
                }

                //Add all top level table joins first
                //NB: there MUST a 1:1 mapping between report base entity names and other top level entities or this will fail
                foreach (var include in includes)
                {
                    var reportEntityJoin = oscarContext.ReportentityJoins
                        .SingleOrDefault(
                            x =>
                            x.BaseEntityName.Equals(reportDto.BaseEntityName) &&
                            x.JoinEntityName.Equals(include)
                        );
                    if (reportEntityJoin != null)
                    {
                        joinClause += reportEntityJoin.JoinExpresssion;
                    }
                    else
                    {
                        var reportEntityJoinForInclude = oscarContext.ReportentityJoins
                         .SingleOrDefault(
                             x =>
                             x.JoinEntityName.Equals(include)
                         );
                        if(reportEntityJoinForInclude != null ) joinClause += reportEntityJoinForInclude.JoinExpresssion;
                    }

                }
            }

            return joinClause;
        }

        internal static string? BuildSelectClauseFromReportFields(ReportDto reportDto)
        {
            string selectClause = "";

            var allFields = reportDto.ReportFields;

            var i = 1;

            if (allFields != null)
            {
                foreach (var field in allFields)
                {
                    if (field.DataType != 1)
                    { 
                        selectClause += "Replace(" +
                            field.BaseEntityName + "." + field.ReportFieldName + " , ',' ,';')"
                            + " as '" + field.BaseEntityName + "." + field.ReportFieldName + "'  ";
                        
                    }
                    else
                    { 
                        var enumType = GetEnumType(ENUM_ASSEMBLY_REF + field.ReportFieldName);

                        selectClause +=

                        " CASE " + field.BaseEntityName + "." + field.ReportFieldName;

                        foreach (int enumValue in Enum.GetValues(enumType))
                        {
                            selectClause +=
                            " WHEN " + enumValue + " THEN '" + Enum.GetName(enumType, enumValue) + "'";
                        }
                        
                        selectClause +=
                            " ELSE '' " +
                        " END " +
                        "as '" + field.BaseEntityName + "." + field.ReportFieldName + "' ";
                    }

                        if (i > 0 && i < allFields.Count) { selectClause += " , "; }
                    i++;
                }
            }

            return selectClause;
        }

        public static Type GetEnumType(string enumName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(enumName);
                if (type == null)
                    continue;
                if (type.IsEnum)
                    return type;
            }
            return null;
        }

        internal static string? BuildWhereClauseFromSearchObjects(List<SearchObject> searchObjects)
        {
            var whereExpression = new StringBuilder(@"");


            int i = 0;
            foreach (var searchObject in searchObjects)
            {
                if (i > 0) { whereExpression.Append(" AND "); }

                whereExpression.Append(searchObject.SearchColumnType switch
                {
                    "boolean" => searchObject.SearchEntity + "." + searchObject.SearchColumn + " = " + searchObject.SearchText + " ",
                    "number" => searchObject.SearchEntity + "." + searchObject.SearchColumn + " = " + searchObject.SearchText + " ",
                    "number_in" => searchObject.SearchEntity + "." + searchObject.SearchColumn + " IN( " + searchObject.SearchText + ") ",
                    _ => searchObject.SearchEntity + "." + searchObject.SearchColumn + " LIKE '%" + searchObject.SearchText + "%' ",
                });
                i++;
            }

            return whereExpression.ToString();
        }

        public static string? BuildQueryFromReportFieldsAndSearchObjects(ReportDto reportDto, List<SearchObject> searchObjects, OscarContext oscarContext)
        {

            var selectClause = ReportHelperService.BuildSelectClauseFromReportFields(reportDto);
            var joinClause = ReportHelperService.BuildJoinClauseFromReportFields(reportDto, searchObjects, oscarContext);
            var whereClause = ReportHelperService.BuildWhereClauseFromSearchObjects(searchObjects);

            var queryString =
                " SELECT CAST(( select distinct " +
                selectClause +
                " from  " + reportDto.BaseEntityName +
                joinClause;

            if (searchObjects.Count > 0) { queryString += " WHERE " + whereClause; }

            queryString += " FOR JSON AUTO) AS VARCHAR(MAX)) AS JSONDATA";

            Console.WriteLine("queryString: " + queryString);

            return queryString;
        }

        internal static void DeleteReport(Core.Entities.Report report, OscarContext oscarContext)
        {

            if (report != null)
            {
                RemoveCollection<ReportField>(report.ReportFields, oscarContext);
                oscarContext.Reports.Remove(report);
            }

        }

        internal static void RemoveCollection<T>(ICollection<T>? childCollection, OscarContext context) where T : BaseEntity
        {
            if (childCollection != null && childCollection.Any())
            {
                context.Set<T>().RemoveRange(childCollection);
            }
        }

        public static string? ConvertJsonToCsv(string jsonString)
        {
            var csvColumns = new List<string> { };
            var csvRows = new List<string> { };

            try
            {

                var objArray = JArray.Parse(jsonString);

                foreach (JObject obj in objArray)
                {
                    // Taken from https://stackoverflow.com/a/32967835
                    // Collect column titles: all property names whose values are of type JValue, distinct, in order of encountering them.
                    var values = obj.DescendantsAndSelf()
                        .OfType<JProperty>()
                        .Where(p => p.Value is JValue)
                        .GroupBy(p => p.Name)
                    .ToList();

                    foreach (var column in values)
                        if (!csvColumns.Contains(column.Key))
                            csvColumns.Add(column.Key);

                    // Filter JObjects that have child objects that have values.
                    var parentsWithChildren = values.SelectMany(g => g).SelectMany(v => v.AncestorsAndSelf().OfType<JObject>().Skip(1)).ToHashSet();

                    // Collect all data rows: for every object, go through the column titles and get the value of that property in the closest ancestor or self that has a value of that name.
                    var rows = obj
                        .DescendantsAndSelf()
                        .OfType<JObject>()
                        .Where(o => o.PropertyValues().OfType<JValue>().Any())
                        .Where(o => o == obj || !parentsWithChildren.Contains(o)) // Show a row for the root object + objects that have no children.
                        .Select(o => csvColumns.Select(c => o.AncestorsAndSelf()
                            .OfType<JObject>()
                            .Select(parent => parent[c])
                            .Where(v => v is JValue)
                            .Select(v => CleanStringForCsvField((string)v))
                            .FirstOrDefault())
                            .Reverse() // Trim trailing nulls
                            .SkipWhile(s => s == null)
                            .Reverse());

                    csvRows.AddRange(rows.ToList().Select(r => string.Join(",", r)));

                }
                
                var csv = string.Join(",", csvColumns) + "\n" + string.Join("\n", csvRows);
                return csv;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Reporting Service ConvertJsonToCsv: " + ex.ToString());
                return null;
            }

        }

        private static string CleanStringForCsvField(string stringToClean)
        {
            if (String.IsNullOrEmpty(stringToClean))
            {
                return stringToClean;
            }
            string lineSeparator = ((char)0x2028).ToString();
            string paragraphSeparator = ((char)0x2029).ToString();

            return stringToClean.Replace("\r\n", string.Empty)
                        .Replace("\n", string.Empty)
                        .Replace("\r", string.Empty)
                        .Replace(lineSeparator, string.Empty)
                        .Replace(paragraphSeparator, string.Empty);
        }

    }
}


