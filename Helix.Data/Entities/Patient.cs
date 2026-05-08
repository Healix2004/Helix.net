using Helix.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Data.Entities
{
    public class Patient : BaseEntity
    {
        // Relation With AppUser
        public virtual AppUser AppUser { get; set; }
        public  EnPatientCategories PatientCategory { get; set; }
        public EnBloodTypes? BloodType { get; set; }
        public List<Allergy> Allergies { get; set; }
        public List<Encounter> Encounters { get; set; }
        public List<Diagnose> Diagnose { get; set; }
        public List<LabTestResult> LabTestResult { get; set; } = new List<LabTestResult>();
        //one to many with consent
        public List<Consent> Consents { get; set; }=new List<Consent>();
    }
}
