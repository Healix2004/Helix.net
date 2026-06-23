using Helix.Data.Enums;

namespace Helix.Service.DTOs.AllergyDTOs
{
    public class AllergyCreateDto
    {
        public string AllergenCatalogId { get; set; }

        // Optional FHIR details the patient/doctor might fill out during registration
        public EnAllergyCategory? Category { get; set; }
        public EnAllergyCriticality? Criticality { get; set; }
        public EnClinicalStatus? ClinicalStatus { get; set; }
        public EnAllergyReaction? Reaction { get; set; }
        public EnAllergySeverity? Severity { get; set; }
    }
}
