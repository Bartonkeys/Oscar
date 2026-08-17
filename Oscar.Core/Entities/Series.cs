namespace Oscar.Core.Entities;

public class Series : Works
{

    public ICollection<Season>? Seasons { get; set; }
    public ICollection<Episode>? Episodes { get; set; }
}