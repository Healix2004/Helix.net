using Helix.Data.Enums;
using System;

namespace Helix.Data.Entities
{
    public class RadiologyOrder : BaseEntity
    {
        // --- Core Data ---
        public string QrToken { get; set; } = default!;
        public EnRadiologyOrderStatus Status { get; set; } = EnRadiologyOrderStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ReasonForExam { get; set; }

        // --- Foreign Keys ---
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid MedicalConceptId { get; set; }

        // --- Navigation Properties ---
        public virtual Patient Patient { get; set; } = null!;
        public virtual Doctor Doctor { get; set; } = null!;
        public virtual MedicalConcept MedicalConcept { get; set; } = null!;
        public virtual RadiologyReport? Report { get; set; }
    }
}