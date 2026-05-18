using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Data.Entities
{
    public class Observation :BaseEntity
    {
        public string? name { get; set; }
        public decimal? value { get; set; }
        public string? unit { get; set; }

        //encounter Relationship
        public Guid PatientId { get; set; }
        public Guid EncounterId { get; set; }
        public Encounter Encounter { get; set; }
        public Patient Patient { get; set; }
    }
}
