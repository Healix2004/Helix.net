using Helix.Data.Enums;
using System;

namespace Helix.Data.Entities
{
    public class LabTestResult : BaseEntity
    {
        // Foreign Keys
        public Guid MedicalConceptId { get; set; }
        public Guid PatientId { get; set; }
        public Guid LabOrderId { get; set; }
        public Guid? EncounterId { get; set; }

        // Navigation Properties
        public virtual MedicalConcept MedicalConcept { get; set; } = null!;
        public virtual Patient Patient { get; set; } = null!;
        public virtual LabOrder LabOrder { get; set; } = null!;
        public virtual Encounter? Encounter { get; set; }

        // Data Fields
        public EnLabOrderStatus Status { get; set; } = EnLabOrderStatus.Pending;
        public decimal? NumericValue { get; set; }
        public string? StringValue { get; set; }
        public string? Unit { get; set; }
        public string? ReferenceRange { get; set; }
        public string? InterpretationFlag { get; set; }
        public DateTime? ResultDate { get; set; } = DateTime.UtcNow;
    }
}