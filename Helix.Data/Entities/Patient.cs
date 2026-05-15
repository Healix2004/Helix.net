using Helix.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Data.Entities
{
    public class Patient : BaseEntity
    {
        // Relation With AppUser
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public EnPatientCategories PatientCategory { get; set; }
        public EnBloodTypes? BloodType { get; set; }
        public List<Allergy> Allergies { get; set; }
        public List<Encounter> Encounters { get; set; }
        public List<Diagnose> Diagnose { get; set; }
        //one to many with consent
        public List<LabTestResult> LabTestResult { get; set; } = new List<LabTestResult>();
        public List<RadiologyResult> RadioTestResult { get; set; } = new List<RadiologyResult>();
        public List<RadiologyOrder> RadiologyOrders { get; set; } = new List<RadiologyOrder>();
        public List<LabOrder> LabOrders { get; set; }= new List<LabOrder>();
        public List<Consent> Consents { get; set; }=new List<Consent>();
    }
}
