namespace Oscar.Core.Entities;

public class Season: Works
{
    public int? SeriesId { get; set; }
    public Series? Series { get; set; }
    
    public ICollection<Episode>? Episodes { get; set; }
}