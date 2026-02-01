using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Helix.Data.Entities
{
    public class Allergy : BaseEntity
    {
        public string name { get; set; }
        public string Category { get; set; }
        public DateTime RecordedDate { get; set; }

        public string criticality { get; set; }

        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }
    }
}
