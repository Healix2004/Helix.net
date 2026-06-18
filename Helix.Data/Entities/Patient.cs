using Helix.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Helix.Data.Entities
{
    public class Patient : BaseEntity
    {
        // --- 1. Identity Link ---
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public EnPatientCategories? PatientCategory { get; set; }

        // --- Step 2: Medical Information ---
        public EnBloodTypes? BloodType { get; set; }
        public string ChronicDiseases { get; set; }
        public string PastSurgeries { get; set; }

        // --- Step 3: Emergency & Insurance ---
        public string EmergencyContactName { get; set; }
        public string EmergencyPhone { get; set; }
        public string EmergencyContactRelation { get; set; }
        public string? InsuranceProvider { get; set; }
        public string? InsurancePolicyNumber { get; set; }

        // --- Navigation Properties (Relationships) ---
        public List<Allergy> Allergies { get; set; } = new List<Allergy>();
        public List<Encounter> Encounters { get; set; } = new List<Encounter>();
        public List<Diagnose> Diagnose { get; set; } = new List<Diagnose>();
        public List<LabTestResult> LabTestResult { get; set; } = new List<LabTestResult>();
        public List<RadiologyResult> RadioTestResult { get; set; } = new List<RadiologyResult>();
        public List<RadiologyOrder> RadiologyOrders { get; set; } = new List<RadiologyOrder>();
        public List<LabOrder> LabOrders { get; set; } = new List<LabOrder>();
        public List<Consent> Consents { get; set; } = new List<Consent>();
    }
}