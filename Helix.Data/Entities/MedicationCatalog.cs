using System.ComponentModel.DataAnnotations;

namespace Helix.Data.Entities
{
    public class MedicationCatalog
    {
        [Key]
        [MaxLength(50)]
        public string Rxcui { get; set; }

        [Required]
        [MaxLength(1000)]
        public string DrugName { get; set; }

        [MaxLength(20)]
        public string TermType { get; set; }

        // Added to store the exact string the Python AI expects
        [MaxLength(200)]
        public string? AiModelName { get; set; }
    }
}