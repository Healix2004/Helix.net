using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Data.Entities
{
    public  class LabTestResult : BaseEntity
    {
        public string status { get; set; }
        public string name { get; set; }
        public decimal value { get; set; }

        //encounter relationship  
        public Guid EncounterId { get; set; }
        public Encounter Encounter { get; set; }
    }
}
