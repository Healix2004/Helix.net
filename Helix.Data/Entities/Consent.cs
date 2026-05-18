using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Data.Entities
{
    public class Consent : BaseEntity
    {
        public DateTime ConsentGrantedAt { get; set; } = DateTime.Now;
        public DateTime ConsentExpiresAt { get; set; } = DateTime.Now.AddHours(5);
        public Guid PatientId { get; set; }
        public Guid DoctorId {  get; set; }
        public bool IsEmergencyOverride { get; set; }=false;
        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
    }
}
