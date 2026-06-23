using System;

namespace Helix.Service.DTOs.AllergyDTOs
{
    public class AllergyDto
    {
        public Guid Id { get; set; }

        // --- Allergy Details (Enums mapped as strings for the frontend) ---
        public string Category { get; set; }
        public string Criticality { get; set; }
        public DateTime RecordedDate { get; set; }
        public string ClinicalStatus { get; set; }
        public string Reaction { get; set; }
        public string Severity { get; set; }

        // --- Patient Info ---
        public Guid PatientId { get; set; }

        // --- Allergen Info ---
        public Guid AllergyCatalogId { get; set; } 
        public string AllergenName { get; set; } 
    }
}
