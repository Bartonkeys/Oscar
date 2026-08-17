namespace Oscar.Core.Entities;

public class Language: LookUpEntity
{
    public ICollection<Works> Works { get; set; }
    public ICollection<LanguageRights> LanguageRights { get; set; }
}