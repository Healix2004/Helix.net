using Helix.Data.Enums;
using System;
using System.Collections.Generic;

namespace Helix.Data.Entities
{
    public class LabOrder : BaseEntity
    {
        public LabOrder()
        {
            Results = new HashSet<LabTestResult>();
        }

        public string QrToken { get; set; }
        public EnLabOrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid PatientId { get; set; } = Guid.Empty;
        public Guid DoctorId { get; set; } = Guid.Empty;

        // Renamed to 'Id' because it holds a Guid, not the string code
        public Guid MedicalConceptId { get; set; } = Guid.Empty;
        public Guid? PrescriptionId { get; set; }

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }

        // PascalCase for public C# properties
        public MedicalConcept MedicalConcept { get; set; }
        public Prescription? Prescription { get; set; }
        public ICollection<LabTestResult> Results { get; set; }
    }
}