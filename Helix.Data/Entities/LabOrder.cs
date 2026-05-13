using Helix.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Data.Entities
{
    public class LabOrder :BaseEntity
    {
        public string QrToken { get; set; } // Unique string e.g., Guid
        public EnLabOrderStatus Status { get; set; } // "Pending", "InProgress", "Completed"
        public DateTime CreatedAt { get; set; }
        public Guid? LabResultId { get; set; }
        public Guid PatientId { get; set; }= Guid.Empty;
        public Guid DoctorId { get; set; } = Guid.Empty;
        public Guid TerminologyCodeId { get; set; } = Guid.Empty;

        // Navigation properties
        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
        public TerminologyCodeLookup TerminologyCode { get; set; }
        public LabTestResult Result { get; set; }
    }
}
