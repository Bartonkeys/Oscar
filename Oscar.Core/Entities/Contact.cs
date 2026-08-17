using System.ComponentModel.DataAnnotations;

namespace Oscar.Core.Entities
{
    public class Contact : BaseEntity
    {
        [MaxLength(10)]
        public string? Title { get; set; }

        [MaxLength(10)]
        public string? Salutation { get; set; }

        [MaxLength(20)]
        public string? FirstName { get; set; }

        [MaxLength(20)]
        public string LastName { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(50)]
        public string? Mobile { get; set; }

        [MaxLength(60)]
        public string? Email { get; set; }

        public string? Comments { get; set; }

        public Address? Address { get; set; }

        public string? Website { get; set; }

        public string? Type { get; set; }

        public string? JobTitle { get; set; }



    }

}