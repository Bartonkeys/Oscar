using Oscar.Core.Enums;

namespace Oscar.Core.Entities;

public class Conflict: BaseEntity
{
    public string Notes { get; set; }
    public DateTime? NotationDate { get; set; }
    public Works? Works { get; set; }
    public Society? Society { get; set; }
    public bool Internal { get; set; }
}