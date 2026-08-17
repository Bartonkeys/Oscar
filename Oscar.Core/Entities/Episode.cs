namespace Oscar.Core.Entities;

public class Episode : Works
{
    public int? SeasonId { get; set; }
    public Season? Season { get; set; }
    public int? SeriesId { get; set; }
    public Series? Series { get; set; }
}