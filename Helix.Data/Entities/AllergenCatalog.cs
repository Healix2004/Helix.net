using System.ComponentModel.DataAnnotations;

namespace Helix.Data.Entities
{
    public class AllergenCatalog
    {
        [Key]
        [MaxLength(50)]
        public string Code { get; set; } // e.g., "256259004"
        [Required]
        public string DisplayName { get; set; } // e.g., "Peanut"
        [MaxLength(50)]
        public string CodeSystem { get; set; } = "SNOMED";
    }
}
