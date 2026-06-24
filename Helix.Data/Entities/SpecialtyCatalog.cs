using System.ComponentModel.DataAnnotations;

namespace Helix.Data.Entities
{
    public class SpecialtyCatalog
    {
        [Key]
        [MaxLength(50)]
        public string Code { get; set; } // The SNOMED Code (e.g., 394579002)

        [Required]
        [MaxLength(200)]
        public string DisplayName { get; set; } // e.g., "Cardiology"

        [MaxLength(50)]
        public string CodeSystem { get; set; } = "SNOMED";
    }
}
