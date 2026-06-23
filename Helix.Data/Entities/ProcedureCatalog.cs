using System.ComponentModel.DataAnnotations;

namespace Helix.Data.Entities
{
    // --- 1. The Lookup Table (Master Dictionary) ---
    public class ProcedureCatalog
    {
        [Key]
        [MaxLength(50)]
        public string Code { get; set; }

        [Required]
        [MaxLength(500)]
        public string DisplayName { get; set; }

        [MaxLength(50)]
        public string CodeSystem { get; set; } = "SNOMED";
    }
}