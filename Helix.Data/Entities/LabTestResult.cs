using Helix.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Data.Entities
{
    public  class LabTestResult : BaseEntity
    {
        // Foreign Keys
        public Guid TerminologyCodeId { get; set; }
        public Guid PatientId { get; set; }
        public Guid OrderId { get; set; }
        public Guid? EncounterId { get; set; }

        // Navigation Properties
        public virtual TerminologyCodeLookup TerminologyCode { get; set; } = null!;
        public virtual Patient Patient { get; set; } = null!;
        public virtual LabOrder LabOrder { get; set; } = null!;
        public virtual Encounter? Encounter { get; set; }

        // Data Fields
        public EnLabOrderStatus Status { get; set; } = EnLabOrderStatus.Pending;
        public decimal Value { get; set; }
        public string Unit { get; set; }
        public DateTime? ResultDate { get; set; } = DateTime.Now;
    }
}
