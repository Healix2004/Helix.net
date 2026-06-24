using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Data.Entities
{
    public class Encounter :BaseEntity
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string status { get; set; }
        
        //patient relation
        public Guid PatientId { get; set; }
        public Patient patient  { get; set; }

        //condition RelationShip
        public List<condition> conditions { get; set; }

        //observation Relationship
        public List<Observation> Observations { get; set; }
    }
}
