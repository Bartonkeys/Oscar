using Oscar.Core.Enums;

namespace Oscar.Core.DTOs
{
    public record RegistrationWorksDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CompactRef { get; set; }
        public HashSet<RegistrationWorksDto> Children = new HashSet<RegistrationWorksDto>();
        public Discriminator Discriminator { get; set; }
        public bool IsChecked { get; set; } = false;
        public bool IsExpanded { get; set; } = false;
        public bool HasChild => Children != null && Children.Count > 0;
        public RegistrationWorksDto Parent { get; set; }
        public bool HasPartialChildSelected()
        {
            int childrenCheckedCount = (from c in Children where c.IsChecked select c).Count();
            return HasChild && childrenCheckedCount > 0 && childrenCheckedCount < Children.Count();
        }

    }
}
