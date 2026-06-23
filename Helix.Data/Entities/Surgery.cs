using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helix.Data.Entities
{
    public class Surgery : BaseEntity
    {
        [Required]
        public DateTime DateOfSurgery { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string SurgeonName { get; set; } = "Unknown";

        [MaxLength(100)]
        public string HospitalOrClinicName { get; set; } = "Unknown";

        public string MedicalNotes { get; set; } = "No additional notes.";

        // --- Navigation Properties ---

        // Link to Patient
        public Guid PatientId { get; set; }
        public virtual Patient Patient { get; set; }

        // Link to Procedure Catalog (FHIR Name)
        [Required]
        [MaxLength(50)]
        public string ProcedureCatalogCode { get; set; }

        [ForeignKey(nameof(ProcedureCatalogCode))]
        public virtual ProcedureCatalog ProcedureCatalog { get; set; }
    }
}