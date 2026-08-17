namespace Oscar.Core.Common
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreExportAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public class ExportAttribute : Attribute
    {
        public string Alias { get; }
        public bool Grouped { get; }
        public ExportAttribute(string alias, bool grouped = false)
        {
            Alias = alias;
            Grouped = grouped;
        }
    }
}
