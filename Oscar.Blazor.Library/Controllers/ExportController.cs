using System;
using System.Dynamic;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oscar.Core.Common;

namespace Oscar.Blazor.Library.Controllers
{
    public class ExportController : Controller
    {
        public IQueryable ApplyQuery<T>(IQueryable<T> items, IQueryCollection query = null, bool keyless = false)
            where T : class
        {
            if (query != null)
            {
                if (query.ContainsKey("$expand"))
                {
                    var propertiesToExpand = query["$expand"].ToString().Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p);
                    }
                }

                var filter = query.ContainsKey("$filter") ? query["$filter"].ToString() : null;
                if (!string.IsNullOrEmpty(filter))
                {
                    if (keyless)
                    {
                        items = items.ToList().AsQueryable();
                    }

                    items = items.Where(filter);
                }

                if (query.ContainsKey("$orderBy"))
                {
                    items = items.OrderBy(query["$orderBy"].ToString());
                }

                if (query.ContainsKey("$skip"))
                {
                    items = items.Skip(int.Parse(query["$skip"].ToString()));
                }

                if (query.ContainsKey("$top"))
                {
                    items = items.Take(int.Parse(query["$top"].ToString()));
                }

                if (query.ContainsKey("$select"))
                {
                    return items.Select($"new ({query["$select"].ToString()})");
                }
            }

            return items;
        }

        public FileStreamResult ToCSV(IQueryable query, string fileName = null)
        {
            var columns = ExportUtil.GetExportProperties(query.ElementType);
            var sb = new StringBuilder();

            foreach (var item in query)
            {
                var row = new List<string>();

                foreach (var column in columns)
                {
                    var colValue = $"{ExportUtil.GetValue(item, column.Key)}".Trim();
                    var underlyingType = column.Value.PropertyType.IsGenericType &&
                                         column.Value.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                        ? Nullable.GetUnderlyingType(column.Value.PropertyType)
                        : column.Value.PropertyType;

                    var typeCode = Type.GetTypeCode(underlyingType);
                    if (TypeCode.DateTime == typeCode && !string.IsNullOrWhiteSpace(colValue))
                    {
                        var dateVal = (DateTime)ExportUtil.GetValue(item, column.Key);
                        colValue = dateVal.Date.ToString("d");
                    }
                    else if (!ExportUtil.IsNumeric(typeCode))
                    {
                        if (colValue.Contains(" "))
                            colValue = $"\"{$"{ExportUtil.GetValue(item, column.Key)}".Trim()}\"";
                    }
                    row.Add(colValue);
                }

                sb.AppendLine(string.Join(",", row.ToArray()));
            }

            var result = new FileStreamResult(new MemoryStream(UTF8Encoding.Default.GetBytes($"{string.Join(",", columns.Select(c => c.Value.PropertyName))}{Environment.NewLine}{sb}")), "text/csv");
            result.FileDownloadName = (!string.IsNullOrEmpty(fileName) ? fileName : "Export") + ".csv";

            return result;
        }

        /// <summary>
        /// Reference: https://learn.microsoft.com/en-us/dotnet/api/documentformat.openxml.spreadsheet.cell?view=openxml-3.0.1
        /// </summary>
        /// <param name="query"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public FileStreamResult ToExcel(IQueryable query, string fileName = null)
        {
            var columns = ExportUtil.GetExportProperties(query.ElementType);
            var stream = new MemoryStream();

            using (var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet();

                var workbookStylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
                ExportUtil.GenerateWorkbookStylesPartContent(workbookStylesPart);

                var sheets = workbookPart.Workbook.AppendChild(new Sheets());
                sheets.Append(new Sheet { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" });

                workbookPart.Workbook.Save();

                var sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                // Report Group Columns
                bool hasGroupedColumns = false;
                if (TryGetGroupedColumns(query, columns, out Row groupRow))
                {
                    sheetData.AppendChild(groupRow);
                    hasGroupedColumns = true;
                }

                // Report Header Columns
                var headerRow = new Row();

                foreach (var column in columns)
                {
                    // If we have already grouped the column, then skip the column
                    if (hasGroupedColumns && column.Value.Grouped) continue;

                    var cell = new Cell()
                    {
                        CellValue = new CellValue(column.Value.PropertyName),
                        DataType = new EnumValue<CellValues>(CellValues.String),
                        StyleIndex = Convert.ToUInt32(2)
                    };
                    headerRow.Append(cell);
                }

                sheetData.AppendChild(headerRow);

                foreach (var item in query)
                {
                    var row = new Row();

                    foreach (var column in columns)
                    {
                        // If we have already grouped the column, then skip the column
                        if (hasGroupedColumns && column.Value.Grouped) continue;

                        var value = ExportUtil.GetValue(item, column.Key);
                        var stringValue = $"{value}".Trim();

                        var cell = new Cell();

                        var underlyingType = column.Value.PropertyType.IsGenericType &&
                                             column.Value.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                            ? Nullable.GetUnderlyingType(column.Value.PropertyType)
                            : column.Value.PropertyType;

                        var typeCode = Type.GetTypeCode(underlyingType);

                        if (typeCode == TypeCode.DateTime && !string.IsNullOrWhiteSpace(stringValue))
                        {
                            DateTime dtValue = ((DateTime)value).Date;
                            stringValue = dtValue.ToOADate().ToString(CultureInfo.InvariantCulture);
                            if (!string.IsNullOrWhiteSpace(stringValue))
                            {
                                cell.CellValue = new CellValue(stringValue);
                                cell.DataType = new EnumValue<CellValues>(CellValues.Number);
                                cell.StyleIndex = (UInt32Value)1U;
                            }
                        }
                        else if (typeCode == TypeCode.Boolean)
                        {
                            cell.CellValue = new CellValue(stringValue.ToLowerInvariant());
                            cell.DataType = new EnumValue<CellValues>(CellValues.Boolean);
                        }
                        else if (ExportUtil.IsNumeric(typeCode))
                        {
                            if (value != null)
                            {
                                stringValue = Convert.ToString(value, CultureInfo.InvariantCulture);
                            }

                            cell.CellValue = new CellValue(stringValue);
                            cell.DataType = new EnumValue<CellValues>(CellValues.Number);
                        }
                        else
                        {
                            cell.CellValue = new CellValue(stringValue);
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);
                        }

                        row.Append(cell);
                    }

                    sheetData.AppendChild(row);
                }

                workbookPart.Workbook.Save();
            }

            if (stream?.Length > 0)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            var result = new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            result.FileDownloadName = (!string.IsNullOrEmpty(fileName) ? fileName : "Export") + ".xlsx";

            return result;
        }

        public static bool TryGetGroupedColumns(IQueryable query, IEnumerable<KeyValuePair<string, ExportProperty>> columns, out Row groupRow)
        {
            groupRow = new Row();
            if (!columns.Any(x => x.Value.Grouped)) return false;
            
            var recordSet = (IQueryable<object>)query;
            if (!recordSet.Any()) return false;

            // We are not ready to display more than 1 client in grouping, hence commented to implement properly when required
            //string columnName = columns.FirstOrDefault(x=> x.Value.Grouped).Key;
            //var records = recordSet.GroupBy(columnName, "it").Select("new (it.Key as GroupedColumn, it as Records)");
            //foreach (dynamic group in records)
            //{
            //    foreach (dynamic record in group.Records)
            //    {
            //        // Process records here ...
            //    }
            //}

            var record = recordSet.FirstOrDefault();
            if (record != null)
            {
                foreach (var gCol in columns.Where(c => c.Value.Grouped))
                {
                    var cellName = new Cell()
                    {
                        CellValue = new CellValue(gCol.Value.PropertyName),
                        DataType = new EnumValue<CellValues>(CellValues.String),
                        StyleIndex = Convert.ToUInt32(0)
                    };
                    var value = ExportUtil.GetValue(record, gCol.Key);
                    var cellValue = new Cell()
                    {
                        CellValue = new CellValue($"{value}".Trim()),
                        DataType = new EnumValue<CellValues>(CellValues.String),
                        StyleIndex = Convert.ToUInt32(2)
                    };
                    groupRow.Append(cellName);
                    groupRow.Append(cellValue);
                }

                return true;
            }

            return false;
        }

    }

    public readonly record struct ExportProperty(string PropertyName, Type PropertyType, bool Grouped = false);

    public static class ExportExcelExtension
    {
        private const string CExcelReplacementChar = "'";
        private const string CRegExPattern = @"[-+=@]";

        public static void ReplaceExcelFormula<T>(this List<T> list)
        {
            if (list == null) return;
            var properties = typeof(T).GetProperties().Where(p => p.PropertyType == typeof(string));

            foreach (var property in properties)
            {
                foreach (var item in list)
                {
                    var existingValue = (string)property.GetValue(item, null)!;
                    if (!string.IsNullOrEmpty(existingValue))
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(existingValue[0].ToString(), CRegExPattern))
                        {
                            if (property.CanWrite)
                            {
                                property.SetValue(item, CExcelReplacementChar + existingValue, null);
                            }
                        }
                    }
                }
            }
        }
    }

    public static class ExportUtil
    {
        public static object GetValue(object target, string name)
        {
            return target.GetType().GetProperty(name).GetValue(target);
        }

        public static IEnumerable<KeyValuePair<string, Type>> GetProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && IsSimpleType(p.PropertyType) && !Attribute.IsDefined(p, typeof(IgnoreExportAttribute)))
                .Select(p => new KeyValuePair<string, Type>(p.Name, p.PropertyType));
        }

        public static IEnumerable<KeyValuePair<string, ExportProperty>> GetExportProperties(Type type)
        {
            var result = new List<KeyValuePair<string, ExportProperty>>();
            var properties = type.GetProperties()
                .Where(p => p.CanRead && IsSimpleType(p.PropertyType) && !Attribute.IsDefined(p, typeof(IgnoreExportAttribute)));

            foreach (var prop in properties)
            {
                var exportAttr = prop.GetCustomAttribute<ExportAttribute>();
                string alias = (exportAttr != null) ? exportAttr.Alias : prop.Name;
                bool grouped = exportAttr?.Grouped ?? false;
                result.Add(new KeyValuePair<string, ExportProperty>(prop.Name, new ExportProperty(alias, prop.PropertyType, grouped)));
            }

            return result;
        }

        public static Dictionary<string, object> GetDynamicProperties(object dynamicObj)
        {
            var properties = new Dictionary<string, object>();

            // Get the 'Properties' property using reflection
            var propInfo = dynamicObj.GetType().GetProperty("Properties", BindingFlags.Public | BindingFlags.Instance);

            if (propInfo != null)
            {
                // Get the dictionary containing the dynamic properties
                var dynamicProperties = propInfo.GetValue(dynamicObj) as Dictionary<string, object>;

                if (dynamicProperties != null)
                {
                    foreach (var kvp in dynamicProperties)
                    {
                        properties[kvp.Key] = kvp.Value;
                    }
                }
            }

            return properties;
        }

        public static bool IsSimpleType(Type type)
        {
            var underlyingType = type.IsGenericType &&
                                 type.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? Nullable.GetUnderlyingType(type)
                : type;

            if (underlyingType == typeof(System.Guid) || underlyingType == typeof(System.DateTimeOffset))
                return true;

            var typeCode = Type.GetTypeCode(underlyingType);

            switch (typeCode)
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Char:
                case TypeCode.DateTime:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.String:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        internal static bool IsNumeric(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        internal static EnumValue<CellValues> GetCellDataType(object value)
        {
            string stringValue = $"{value.ToString()}".Trim();
            if (string.IsNullOrWhiteSpace(stringValue)) return new EnumValue<CellValues>(CellValues.String);

            if (DateTime.TryParse(stringValue, out DateTime dtValue))
            {
                return new EnumValue<CellValues>(CellValues.Number);
            }
            if (int.TryParse(stringValue, out int intValue))
            {
                return new EnumValue<CellValues>(CellValues.Number);
            }
            if (bool.TryParse(stringValue, out bool boolValue))
            {
                return new EnumValue<CellValues>(CellValues.Boolean);
            }
            return  new EnumValue<CellValues>(CellValues.String);
        }

        internal static void GenerateWorkbookStylesPartContent(WorkbookStylesPart workbookStylesPart1)
        {
            Stylesheet stylesheet1 = new Stylesheet { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac x16r2 xr" } };
            stylesheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            stylesheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");
            stylesheet1.AddNamespaceDeclaration("x16r2", "http://schemas.microsoft.com/office/spreadsheetml/2015/02/main");
            stylesheet1.AddNamespaceDeclaration("xr", "http://schemas.microsoft.com/office/spreadsheetml/2014/revision");

            Fonts fonts1 = new Fonts { Count = (UInt32Value)2U, KnownFonts = true };

            Font font1 = new Font();
            FontSize fontSize1 = new FontSize { Val = 11D };
            Color color1 = new Color { Theme = (UInt32Value)1U };
            FontName fontName1 = new FontName { Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering1 = new FontFamilyNumbering() { Val = 2 };
            FontScheme fontScheme1 = new FontScheme { Val = FontSchemeValues.Minor };

            font1.Append(fontSize1);
            font1.Append(color1);
            font1.Append(fontName1);
            font1.Append(fontFamilyNumbering1);
            font1.Append(fontScheme1);

            Font font2 = new Font();
            font2.Append(new Bold());
            font2.Append(new FontSize { Val = 11D });
            font2.Append(new Color { Theme = (UInt32Value)1U });
            font2.Append(new FontName { Val = "Calibri" });

            fonts1.Append(font1);
            fonts1.Append(font2);

            Fills fills1 = new Fills { Count = (UInt32Value)2U };

            Fill fill1 = new Fill();
            PatternFill patternFill1 = new PatternFill { PatternType = PatternValues.None };

            fill1.Append(patternFill1);

            Fill fill2 = new Fill();
            PatternFill patternFill2 = new PatternFill { PatternType = PatternValues.Gray125 };

            fill2.Append(patternFill2);

            fills1.Append(fill1);
            fills1.Append(fill2);

            Borders borders1 = new Borders { Count = (UInt32Value)1U };

            Border border1 = new Border();
            LeftBorder leftBorder1 = new LeftBorder();
            RightBorder rightBorder1 = new RightBorder();
            TopBorder topBorder1 = new TopBorder();
            BottomBorder bottomBorder1 = new BottomBorder();
            DiagonalBorder diagonalBorder1 = new DiagonalBorder();

            border1.Append(leftBorder1);
            border1.Append(rightBorder1);
            border1.Append(topBorder1);
            border1.Append(bottomBorder1);
            border1.Append(diagonalBorder1);

            borders1.Append(border1);

            CellStyleFormats cellStyleFormats1 = new CellStyleFormats { Count = (UInt32Value)1U };
            CellFormat cellFormat1 = new CellFormat
            {
                NumberFormatId = (UInt32Value)0U,
                FontId = (UInt32Value)0U,
                FillId = (UInt32Value)0U,
                BorderId = (UInt32Value)0U
            };

            cellStyleFormats1.Append(cellFormat1);

            CellFormats cellFormats1 = new CellFormats { Count = (UInt32Value)3U };
            CellFormat cellFormat2 = new CellFormat
            {
                NumberFormatId = (UInt32Value)0U,
                FontId = (UInt32Value)0U,
                FillId = (UInt32Value)0U,
                BorderId = (UInt32Value)0U,
                FormatId = (UInt32Value)0U
            };
            CellFormat cellFormat3 = new CellFormat
            {
                NumberFormatId = (UInt32Value)14U,
                FontId = (UInt32Value)0U,
                FillId = (UInt32Value)0U,
                BorderId = (UInt32Value)0U,
                FormatId = (UInt32Value)0U,
                ApplyNumberFormat = true
            };

            CellFormat boldCellFormat = new CellFormat
            {
                FontId = (UInt32Value)1U,
                FillId = (UInt32Value)0U,
                BorderId = (UInt32Value)0U
            };

            cellFormats1.Append(cellFormat2);
            cellFormats1.Append(cellFormat3);
            cellFormats1.Append(boldCellFormat);

            CellStyles cellStyles1 = new CellStyles { Count = (UInt32Value)1U };
            CellStyle cellStyle1 = new CellStyle { Name = "Normal", FormatId = (UInt32Value)0U, BuiltinId = (UInt32Value)0U };
            cellStyles1.Append(cellStyle1);

            DifferentialFormats differentialFormats1 = new DifferentialFormats() { Count = (UInt32Value)0U };
            TableStyles tableStyles1 = new TableStyles
            {
                Count = (UInt32Value)0U,
                DefaultTableStyle = "TableStyleMedium2",
                DefaultPivotStyle = "PivotStyleLight16"
            };

            StylesheetExtensionList stylesheetExtensionList1 = new StylesheetExtensionList();

            StylesheetExtension stylesheetExtension1 = new StylesheetExtension { Uri = "{EB79DEF2-80B8-43e5-95BD-54CBDDF9020C}" };
            stylesheetExtension1.AddNamespaceDeclaration("x14", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main");

            StylesheetExtension stylesheetExtension2 = new StylesheetExtension { Uri = "{9260A510-F301-46a8-8635-F512D64BE5F5}" };
            stylesheetExtension2.AddNamespaceDeclaration("x15", "http://schemas.microsoft.com/office/spreadsheetml/2010/11/main");

            OpenXmlUnknownElement openXmlUnknownElement4 = workbookStylesPart1.CreateUnknownElement(
                "<x15:timelineStyles defaultTimelineStyle=\"TimeSlicerStyleLight1\" xmlns:x15=\"http://schemas.microsoft.com/office/spreadsheetml/2010/11/main\" />");

            stylesheetExtension2.Append(openXmlUnknownElement4);

            stylesheetExtensionList1.Append(stylesheetExtension1);
            stylesheetExtensionList1.Append(stylesheetExtension2);

            stylesheet1.Append(fonts1);
            stylesheet1.Append(fills1);
            stylesheet1.Append(borders1);
            stylesheet1.Append(cellStyleFormats1);
            stylesheet1.Append(cellFormats1);
            stylesheet1.Append(cellStyles1);
            stylesheet1.Append(differentialFormats1);
            stylesheet1.Append(tableStyles1);
            stylesheet1.Append(stylesheetExtensionList1);

            workbookStylesPart1.Stylesheet = stylesheet1;
        }

        public static bool TryGetGroupedColumns<T>(IList<T> items, IEnumerable<KeyValuePair<string, ExportProperty>> columns, out Row groupRow)
        {
            groupRow = new Row();
            if (!columns.Any(x => x.Value.Grouped)) return false;

            if (!items.Any()) return false;

            // We are not ready to display more than 1 client in grouping, hence commented to implement properly when required
            //string columnName = columns.FirstOrDefault(x=> x.Value.Grouped).Key;
            //var records = recordSet.GroupBy(columnName, "it").Select("new (it.Key as GroupedColumn, it as Records)");
            //foreach (dynamic group in records)
            //{
            //    foreach (dynamic record in group.Records)
            //    {
            //        // Process records here ...
            //    }
            //}

            var record = items.FirstOrDefault();
            if (record != null)
            {
                foreach (var gCol in columns.Where(c => c.Value.Grouped))
                {
                    var cellName = new Cell()
                    {
                        CellValue = new CellValue(gCol.Value.PropertyName),
                        DataType = new EnumValue<CellValues>(CellValues.String),
                        StyleIndex = Convert.ToUInt32(0)
                    };
                    var value = GetValue(record, gCol.Key);
                    var cellValue = new Cell()
                    {
                        CellValue = new CellValue($"{value}".Trim()),
                        DataType = new EnumValue<CellValues>(CellValues.String),
                        StyleIndex = Convert.ToUInt32(2)
                    };
                    groupRow.Append(cellName);
                    groupRow.Append(cellValue);
                }

                return true;
            }

            return false;
        }

        public static FileStreamResult ToExcel<T>(IList<T> items, string fileName = null)
        {
            var columns = GetExportProperties(typeof(T));
            var stream = new MemoryStream();

            using (var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet();

                var workbookStylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
                GenerateWorkbookStylesPartContent(workbookStylesPart);

                var sheets = workbookPart.Workbook.AppendChild(new Sheets());
                sheets.Append(new Sheet { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" });

                workbookPart.Workbook.Save();

                var sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                // Report Group Columns
                bool hasGroupedColumns = false;
                if (TryGetGroupedColumns(items, columns, out Row groupRow))
                {
                    sheetData.AppendChild(groupRow);
                    hasGroupedColumns = true;
                }

                // Report Header Columns
                var headerRow = new Row();

                foreach (var column in columns)
                {
                    // If we have already grouped the column, then skip the column
                    if (hasGroupedColumns && column.Value.Grouped) continue;

                    var cell = new Cell()
                    {
                        CellValue = new CellValue(column.Value.PropertyName),
                        DataType = new EnumValue<CellValues>(CellValues.String),
                        StyleIndex = Convert.ToUInt32(2)
                    };
                    headerRow.Append(cell);
                }

                sheetData.AppendChild(headerRow);

                foreach (var item in items)
                {
                    var row = new Row();

                    foreach (var column in columns)
                    {
                        // If we have already grouped the column, then skip the column
                        if (hasGroupedColumns && column.Value.Grouped) continue;

                        var value = GetValue(item, column.Key);
                        var stringValue = $"{value}".Trim();

                        var cell = new Cell();

                        var underlyingType = column.Value.PropertyType.IsGenericType &&
                                             column.Value.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                            ? Nullable.GetUnderlyingType(column.Value.PropertyType)
                            : column.Value.PropertyType;

                        var typeCode = Type.GetTypeCode(underlyingType);

                        if (typeCode == TypeCode.DateTime && !string.IsNullOrWhiteSpace(stringValue))
                        {
                            DateTime dtValue = ((DateTime)value).Date;
                            stringValue = dtValue.ToOADate().ToString(CultureInfo.InvariantCulture);
                            if (!string.IsNullOrWhiteSpace(stringValue))
                            {
                                cell.CellValue = new CellValue(stringValue);
                                cell.DataType = new EnumValue<CellValues>(CellValues.Number);
                                cell.StyleIndex = (UInt32Value)1U;
                            }
                        }
                        else if (typeCode == TypeCode.Boolean)
                        {
                            cell.CellValue = new CellValue(stringValue.ToLowerInvariant());
                            cell.DataType = new EnumValue<CellValues>(CellValues.Boolean);
                        }
                        else if (IsNumeric(typeCode))
                        {
                            if (value != null)
                            {
                                stringValue = Convert.ToString(value, CultureInfo.InvariantCulture);
                            }

                            cell.CellValue = new CellValue(stringValue);
                            cell.DataType = new EnumValue<CellValues>(CellValues.Number);
                        }
                        else
                        {
                            cell.CellValue = new CellValue(stringValue);
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);
                        }

                        row.Append(cell);
                    }

                    sheetData.AppendChild(row);
                }

                workbookPart.Workbook.Save();
            }

            if (stream?.Length > 0)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            var result = new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            result.FileDownloadName = (!string.IsNullOrEmpty(fileName) ? fileName : "Export") + ".xlsx";

            return result;
        }

        public static FileStreamResult ToExcelDynamic<T>(IList<T> items, string fileName = null)
        {
            var stream = new MemoryStream();

            using (var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet();

                var workbookStylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
                GenerateWorkbookStylesPartContent(workbookStylesPart);

                var sheets = workbookPart.Workbook.AppendChild(new Sheets());
                sheets.Append(new Sheet { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" });

                workbookPart.Workbook.Save();

                var sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                bool headerRowCreated = false;
                foreach (var item in items)
                {
                    var columns = GetDynamicProperties(item);
                    if (!headerRowCreated)
                    {
                        // Create Report Header Columns
                        var headerRow = new Row();
                        foreach (var column in columns)
                        {
                            var cell = new Cell()
                            {
                                CellValue = new CellValue(column.Key.Replace('_', ' ').Replace('-',' ')),
                                DataType = new EnumValue<CellValues>(CellValues.String),
                                StyleIndex = Convert.ToUInt32(2)
                            };
                            headerRow.Append(cell);
                        }

                        sheetData.AppendChild(headerRow);
                        headerRowCreated = true;
                    }
                    var row = new Row();

                    foreach (var column in columns)
                    {
                        var stringValue = $"{column.Value}".Trim();

                        var cell = new Cell
                        {
                            CellValue = new CellValue(stringValue),
                            DataType = GetCellDataType(column.Value)
                        };

                        row.Append(cell);
                    }

                    sheetData.AppendChild(row);
                }

                workbookPart.Workbook.Save();
            }

            if (stream?.Length > 0)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            var result = new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            result.FileDownloadName = (!string.IsNullOrEmpty(fileName) ? fileName : "Export") + ".xlsx";

            return result;
        }

        public static FileStreamResult ToCsv<T>(IList<T> items, string fileName = null)
        {
            var columns = ExportUtil.GetExportProperties(typeof(T));
            var sb = new StringBuilder();

            foreach (var item in items)
            {
                var row = new List<string>();

                foreach (var column in columns)
                {
                    var colValue = $"{ExportUtil.GetValue(item, column.Key)}".Trim();
                    var underlyingType = column.Value.PropertyType.IsGenericType &&
                                         column.Value.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                        ? Nullable.GetUnderlyingType(column.Value.PropertyType)
                        : column.Value.PropertyType;

                    var typeCode = Type.GetTypeCode(underlyingType);
                    if (TypeCode.DateTime == typeCode && !string.IsNullOrWhiteSpace(colValue))
                    {
                        var dateVal = (DateTime)ExportUtil.GetValue(item, column.Key);
                        colValue = dateVal.Date.ToString("d");
                    }
                    else if (!ExportUtil.IsNumeric(typeCode))
                    {
                        if (colValue.Contains(" "))
                            colValue = $"\"{$"{ExportUtil.GetValue(item, column.Key)}".Trim()}\"";
                    }
                    row.Add(colValue);
                }

                sb.AppendLine(string.Join(",", row.ToArray()));
            }

            var result = new FileStreamResult(new MemoryStream(UTF8Encoding.Default.GetBytes($"{string.Join(",", columns.Select(c => c.Value.PropertyName))}{Environment.NewLine}{sb}")), "text/csv");
            result.FileDownloadName = (!string.IsNullOrEmpty(fileName) ? fileName : "Export") + ".csv";

            return result;
        }

        public static FileStreamResult ToCsvDynamic<T>(IList<T> items, string fileName = null)
        {
            var sb = new StringBuilder();
            bool headerRowSet = false;
            List<string> headerColumns = new List<string>();
            foreach (var item in items)
            {
                var columns = GetDynamicProperties(item);
                var row = new List<string>();

                foreach (var column in columns)
                {
                    if (!headerRowSet)
                    {
                        headerColumns.Add(column.Key);
                    }
                    var colValue = $"{column.Value}".Trim();
                    if (colValue.Contains(" "))
                    {
                       colValue = $"\"{$"{colValue}".Trim()}\"";
                    }
                    row.Add(colValue);
                }
                headerRowSet = true;
                sb.AppendLine(string.Join(",", row.ToArray()));
            }

            var result = new FileStreamResult(new MemoryStream(UTF8Encoding.Default.GetBytes($"{string.Join(",", headerColumns)}{Environment.NewLine}{sb}")), "text/csv");
            result.FileDownloadName = (!string.IsNullOrEmpty(fileName) ? fileName : "Export") + ".csv";

            return result;
        }
    }

    
}
