using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Data.Entities
{
    public class Doctor : BaseEntity
    {
        // Relation With AppUser
        public virtual AppUser AppUser { get; set; }
        public string Specialty { get; set; } = default!;
        public decimal ConsultationFee { get; set; } = 0;
        public string Bio { get; set; }
        public string SyndicateNumber { get; set; }
        public List<Encounter> Encounters { get; set; }
        public List<Facilitie> Facilities { get; set; }

        //one to many with consent
        public List<Consent> Consents { get; set; }= new List<Consent>();
    }
}
