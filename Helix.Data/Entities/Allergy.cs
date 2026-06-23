using Helix.Data.Enums;
using System;

namespace Helix.Data.Entities
{
    public class Allergy : BaseEntity
    {
        public EnAllergyCategory? Category { get; set; } = EnAllergyCategory.Unknown;
        public EnAllergyCriticality? Criticality { get; set; } = EnAllergyCriticality.Unknown;
        public DateTime RecordedDate { get; set; } = DateTime.UtcNow;
        public EnClinicalStatus? ClinicalStatus { get; set; } = EnClinicalStatus.Active;
        public EnAllergyReaction? Reaction { get; set; } = EnAllergyReaction.Unknown;
        public EnAllergySeverity? Severity { get; set; } = EnAllergySeverity.Unknown;

        // Foreign Key to Patient
        public Guid PatientId { get; set; }
        public virtual Patient Patient { get; set; }

        public string AllergenCatalogCode { get; set; }
        public virtual AllergenCatalog AllergenCatalog { get; set; }
    }
}