using System.ComponentModel.DataAnnotations;

namespace Helix.Data.Entities
{
    public class LoincPanelComponent :BaseEntity
    {
        [Required]
        public Guid ParentLoincConceptId { get; set; } // FK to MedicalConcept (the panel)
        public MedicalConcept ParentLoincConcept { get; set; }

        [Required]
        public Guid ChildLoincConceptId { get; set; } // FK to MedicalConcept (the component)
        public MedicalConcept ChildLoincConcept { get; set; }

        public int Sequence { get; set; } // Order of the component within the panel

        [MaxLength(10)]
        public string Conditionality { get; set; } = string.Empty; // e.g., 'R' for Required, 'O' for Optional

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
