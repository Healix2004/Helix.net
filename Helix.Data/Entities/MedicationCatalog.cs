using System.ComponentModel.DataAnnotations;

namespace Helix.Data.Entities
{
    // This lookup table holds the local RxNorm dataset
    public class MedicationCatalog
    {
        [Key]
        [MaxLength(50)]
        public string Rxcui { get; set; }

        [Required]
        [MaxLength(1000)]
        public string DrugName { get; set; }

        // TTY (Term Type) - tells you if it's an Ingredient (IN), Brand Name (BN), or Clinical Drug (SCD)
        [MaxLength(20)]
        public string TermType { get; set; }
    }
}