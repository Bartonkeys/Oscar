using Oscar.Core.Enums;

namespace Oscar.Core.DTOs
{
    public record ClientUpdateDto : ClientAddDto
    {
        public ICollection<SocietyDto>? Societies { get; set; }
    }
}
