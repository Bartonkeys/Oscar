using Oscar.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oscar.Core.Entities
{
    public class WorksImportRequest: BaseEntity
    {
        public WorksImportRequestStatus Status { get; set; }
        
        [StringLength(30)]
        public string Reference { get; set; }

        [StringLength(30)]
        public string RequestedBy { get; set; }

        [ForeignKey("Catalogue")]
        public int? CatalogueId { get; set; }

        public Client Client { get; set; }
        public Catalogue? Catalogue { get; set; }

        public ICollection<WorksImport>? WorksImports { get; set; }
    }

}
