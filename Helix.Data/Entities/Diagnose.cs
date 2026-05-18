using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Data.Entities
{
    public class Diagnose : BaseEntity
    {
        public Guid TerminologyCodeLookupId { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId {  get; set; }
        public TerminologyCodeLookup TerminologyCode{ get; set; }
        public Patient Patient { get; set; }
        public Doctor doctor { get; set; }
        public DateOnly dateOnly { get; set; }
        public string Notes { get; set; }
        public EnStatus status { get; set; }
    }
}
