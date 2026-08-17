namespace Oscar.Core.DTOs
{
    public class CountriesGroupsDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; } = false;
        public bool IsExpanded { get; set; }
        public bool HasChild => Children != null && Children.Count > 0;
        public CountriesGroupsDto Parent { get; set; } = null;
        public HashSet<CountriesGroupsDto> Children { get; set; } = new HashSet<CountriesGroupsDto>();

        public bool HasPartialChildSelected()
        {
            int childrenCheckedCount = (from c in Children where c.IsChecked select c).Count();
            return HasChild && childrenCheckedCount > 0 && childrenCheckedCount < Children.Count();
        }
    }
}
