using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Data.Entities
{
    public class Medication:BaseEntity
    {
        public Guid PatientId { get; set; }
        public Guid TerminologyCodeId { get; set; }
        public TerminologyCodeLookup TerminologyCode{ get; set; }
        public Patient Patient { get; set; }
        public string Dosage {  get; set; }
        public String Frequency { get; set; }
        public DateOnly StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
