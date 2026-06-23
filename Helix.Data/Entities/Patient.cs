using Helix.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Helix.Data.Entities
{
    public class Patient : BaseEntity
    {
        // --- 1. Identity Link ---
        public string AppUserId { get; set; }
        public virtual AppUser AppUser { get; set; }
        public EnPatientCategories? PatientCategory { get; set; }

        // --- Step 2: Medical Information ---
        public EnBloodTypes? BloodType { get; set; }

        // --- Step 3: Emergency & Insurance ---
        public Insurance? Insurance { get; set; }

        // --- Navigation Properties (Relationships) ---

        // Links to the FHIR-compliant Surgery entity we created in ProcedureEntities.cs!
        public List<Surgery> Surgeries { get; set; } = new List<Surgery>();
        public List<Medication> Medications { get; set; } = new List<Medication>();
        public List<ChronicDisease> ChronicDiseases { get; set; } = new List<ChronicDisease>();
        public List<EmergencyContact> EmergencyContacts { get; set; } = new List<EmergencyContact>();
        public List<Allergy> Allergies { get; set; } = new List<Allergy>();
        public List<Encounter> Encounters { get; set; } = new List<Encounter>();
        public List<Diagnose> Diagnose { get; set; } = new List<Diagnose>();
        public List<LabTestResult> LabTestResult { get; set; } = new List<LabTestResult>();
        public List<RadiologyResult> RadioTestResult { get; set; } = new List<RadiologyResult>();
        public List<RadiologyOrder> RadiologyOrders { get; set; } = new List<RadiologyOrder>();
        public List<LabOrder> LabOrders { get; set; } = new List<LabOrder>();
        public List<Consent> Consents { get; set; } = new List<Consent>();
    }

    [Owned]
    public class Insurance
    {
        public string? InsuranceProvider { get; set; }
        public string? InsurancePolicyNumber { get; set; }
    }
}