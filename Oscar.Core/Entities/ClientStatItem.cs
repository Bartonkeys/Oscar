using Oscar.Core.Common;
using System.Dynamic;

namespace Oscar.Core.Entities;

public class ClientWorkStatItem
{
    [IgnoreExport]
    public int? ClientId { get; set; }

    [Export("Client")]
    public string? ClientName { get; set; }
    [Export("Grade")]
    public string? ClientGrade { get; set; }
    [Export("CSM")]
    public string? AccountManager { get; set; }
    [Export("Status")]
    public string? ClientStatus { get; set; }
    [Export("Created Year")]
    public int? CreatedYear { get; set; }
    public int? Episodes { get; set; }
    public int? Seasons {get; set; }
    public int? Series { get; set; }
    [Export("Titles")]
    public int? StandAlones{ get; set; }

    public virtual bool FilterBy(string searchText)
    {
        return (string.IsNullOrEmpty(searchText) ||
                (!string.IsNullOrEmpty(ClientName) && ClientName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(ClientGrade) && ClientGrade.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(ClientStatus) && ClientStatus.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(AccountManager) && AccountManager.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (ClientId > 0 && ClientId?.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (CreatedYear > 0 && CreatedYear?.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1)
            );
    }
}

public class ClientWorkStatItemEx
{
    [IgnoreExport]
    public int? ClientId { get; set; }

    [Export("Client")]
    public string? ClientName { get; set; }
    [Export("Grade")]
    public string? ClientGrade { get; set; }
    [Export("CSM")]
    public string? AccountManager { get; set; }
    [Export("Status")]
    public string? ClientStatus { get; set; }
    [Export("Production Year")]
    public int? ProductionYear { get; set; }
    public int? Episodes { get; set; }
    public int? Seasons { get; set; }
    public int? Series { get; set; }
    [Export("Titles")]
    public int? StandAlones { get; set; }

    public virtual bool FilterBy(string searchText)
    {
        return (string.IsNullOrEmpty(searchText) ||
                (!string.IsNullOrEmpty(ClientName) && ClientName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(ClientGrade) && ClientGrade.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(ClientStatus) && ClientStatus.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(AccountManager) && AccountManager.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (ClientId > 0 && ClientId?.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (ProductionYear > 0 && ProductionYear?.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1)
            );
    }
}

public class DynamicEntityItem : DynamicObject
{
    private readonly Dictionary<string, object?> _properties = new Dictionary<string, object?>();

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        return _properties.TryGetValue(binder.Name, out result);
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        _properties[binder.Name] = value;
        return true;
    }

    public Dictionary<string, object?> Properties => _properties;
}