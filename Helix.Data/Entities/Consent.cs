using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Data.Entities
{
    public class Consent : BaseEntity
    {
        public DateTime ConsentGrantedAt { get; set; } = DateTime.Now;
        public DateTime ConsentExpiresAt { get; set; } = DateTime.Now.AddHours(5);

        public bool IsEmergencyOverride { get; set; }=false;
        //one to many with patient
        public Patient Patient { get; set; }
        //one to many with doctor
        public Doctor Doctor { get; set; }
    }
}
