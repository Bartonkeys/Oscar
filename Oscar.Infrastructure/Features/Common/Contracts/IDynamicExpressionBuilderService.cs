using System;
using Oscar.Core.DTOs;

namespace Oscar.Infrastructure.Features.Common.Contracts
{
	public interface IDynamicExpressionBuilderService
	{
        string ReplaceKnownColumnNames(string columnName, Dictionary<string, string> columnNameDictionary);

        string GenerateDynamicWhereExpression(BaseTableQuery baseTableQuery, Dictionary<string, string> columnNameDictionary);

        string GenerateDynamicSelectExpression(List<SelectObject> selectObjects, ReportDto reportDto);
    }
}


