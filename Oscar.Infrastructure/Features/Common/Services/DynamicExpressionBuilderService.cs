using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Oscar.Core.Enums;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using System.Text;
using Oscar.Infrastructure.Features.Common.Contracts;


namespace Oscar.Infrastructure.Features.Common.Services
{
	public class DynamicExpressionBuilderService : IDynamicExpressionBuilderService
	{

        public string ReplaceKnownColumnNames(string columnName, Dictionary<string, string> columnNameDictionary)
        {
            return columnNameDictionary.FirstOrDefault(x => x.Value == columnName).Key ?? columnName;
        }

        public string GenerateDynamicWhereExpression(BaseTableQuery baseTableQuery, Dictionary<string, string> columnNameDictionary)
        {
            var whereExpression = new StringBuilder("");
            var searchObjects = baseTableQuery.SearchObjects;

            if (searchObjects.Count() <= 0)
            {
                return whereExpression.Append($"c != null").ToString();
            }

            int i = 0;
            foreach (var searchObject in searchObjects)
            {
                if (i > 0) { whereExpression.Append(" && "); }

                if (baseTableQuery.BaseEntityName == "Works" && searchObject.SearchColumn == "discriminator")
                {
                    whereExpression.Append($"c.{ReplaceKnownColumnNames(searchObject.SearchColumn, columnNameDictionary)} == \"{searchObject.SearchText}\"");
                    i++;
                    continue;
                }

                // check if search object entity equals base entity
                // if not then we need to check for type of search object
                // If it is a collection then we need to use the format:
                // c.[ENTITY_NAME].Any(x => Append($"x.{ReplaceKnownColumnNames(searchObject.SearchColumn, columnNameDictionary)} == {searchObject.SearchText}"))
                // prob need to refactor switch out for generic with entity lambda identifier (i.e. 'c' or 'x')

                if (searchObject.SearchEntity == baseTableQuery.BaseEntityName)
                {
                    switch (searchObject.SearchColumnType)
                    {
                        case "boolean":
                            whereExpression.Append($"c.{ReplaceKnownColumnNames(searchObject.SearchColumn, columnNameDictionary)} == {searchObject.SearchText}");
                            break;
                        case "number":
                            whereExpression.Append($"c.{ReplaceKnownColumnNames(searchObject.SearchColumn, columnNameDictionary)} == {searchObject.SearchText}");
                            break;
                        case "string":
                        default:
                            whereExpression.Append($"c.{ReplaceKnownColumnNames(searchObject.SearchColumn, columnNameDictionary)}.Contains( \"{searchObject.SearchText}\")");
                            break;
                    }
                }
                else
                {
                    switch (searchObject.SearchColumnType)
                    {
                        case "boolean":
                            whereExpression.Append($"c.{searchObject.SearchEntity}.Any(x => x.{ReplaceKnownColumnNames(searchObject.SearchColumn, columnNameDictionary)} == {searchObject.SearchText})");
                            break;
                        case "number":
                            whereExpression.Append($"c.{searchObject.SearchEntity}.Any(x => x.{ReplaceKnownColumnNames(searchObject.SearchColumn, columnNameDictionary)} == {searchObject.SearchText})");
                            break;
                        case "string":
                        default:
                            whereExpression.Append($"c.{searchObject.SearchEntity}.Any(x => x.{ReplaceKnownColumnNames(searchObject.SearchColumn, columnNameDictionary)}.Contains( \"{searchObject.SearchText}\"))");
                            break;
                    }
                }
                

                i++;
            }

            return whereExpression.ToString();
        }

        public string GenerateDynamicSelectExpression(List<SelectObject> selectObjects, ReportDto reportDto)
        {
            var selectExpression = new StringBuilder("new { ");

            int i = 0;
            var groupedByTable = selectObjects.GroupBy(x => x.SelectTable)
                .Select(x => (SelectTable: x.Key, SelectField: x.Select(p => p.SelectField).ToList()));



            foreach (var selectObjectGroup in groupedByTable)
            {
                if (i > 0) { selectExpression.Append(" , "); }
                if (selectObjectGroup.SelectTable == reportDto.BaseEntityName)
                {
                    int j = 0;
                    foreach (var selectObject in selectObjectGroup.SelectField)
                    {
                        if (j > 0) { selectExpression.Append(" , "); }
                        selectExpression.Append($"c.{selectObject}");
                        j++;
                    }

                }
                else
                {
                    int j = 0;
                    selectExpression.Append($"{selectObjectGroup.SelectTable} = c.{selectObjectGroup.SelectTable}.Select(w => new {{");

                    foreach (var selectObject in selectObjectGroup.SelectField)
                    {
                        if (j > 0) { selectExpression.Append(" , "); }
                        selectExpression.Append($"w.{selectObject}");
                        j++;
                    }

                    selectExpression.Append(" })");
                }

                i++;
            }
            selectExpression.Append(" }");

            return selectExpression.ToString();
        }
    }
}

