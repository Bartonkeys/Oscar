using System.ComponentModel.DataAnnotations;
using Oscar.Core.Enums;

namespace Oscar.Core.Entities
{
    public class WorksTitle: BaseEntity, ITitle
    {
        private string? _title;
        public string? Title
        {
            get { return _title; }
            set
            {
                _title = !string.IsNullOrEmpty(value) ? value.ToUpper() : value;
            }
        }

        public string? ReverseTitle { get; set; }

        [MaxLength(10)]
        public string? LanguageCode { get; set; }

        public TitleType TitleType { get; set; }

        public Works Works { get; set; }
    }

    public interface ITitle
    {
        public string? Title { get; set; }
    }
}
