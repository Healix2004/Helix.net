using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Data.Entities
{
    public  class LabTestResult : BaseEntity
    {
        public TerminologyCodeLookup TerminologyCode{ get; set; }
        public Guid PatientId { get; set; } = Guid.Empty;
        public Guid DoctorId { get; set; } = Guid.Empty;
        public Guid OrderId { get; set; } = Guid.Empty;
        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }// who doctor request
        public EnStatus status { get; set; }
        public decimal? value { get; set; }
        public string? Unit { get; set; }
        public DateTime? ResultDate { get; set; }
        public List<LabImages> images { get; set; }
        public Encounter Encounter { get; set; }
        public LabOrder LabOrder { get; set; } = new LabOrder();
    }
}
