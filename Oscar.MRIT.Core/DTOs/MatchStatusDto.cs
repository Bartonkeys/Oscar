using Oscar.MRIT.Core.Enums;

namespace Oscar.MRIT.Core.DTOs
{
    public class MatchStatusDto
    {
        public int WorksId { get; set; }
        public MatchStatus MatchStatus { get; set; }
        public string Message { get; set; }
    }
}
