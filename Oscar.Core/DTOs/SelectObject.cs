using System;
namespace Oscar.Core.DTOs
{
    public class SelectObject
    {
        public SelectObject(string? SelectTable, string? SelectField)
        {
            this.SelectField = SelectField;
            this.SelectTable = SelectTable;
        }

        public string? SelectTable { get; set; } = "";
        public string? SelectField { get; set; } = "";

    }
}

