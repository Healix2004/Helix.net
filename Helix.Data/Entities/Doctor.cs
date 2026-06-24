using Helix.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Timers;

namespace Helix.Data.Entities
{
    public class Doctor : BaseEntity
    {
        // --- 1. Identity Link ---
        public string AppUserId { get; set; }
        public virtual AppUser AppUser { get; set; }

        // --- Step 2: Professional Information ---
        public string FullName { get; set; } = default!;
        public string NationalId { get; set; } = default!;
        public string SpecialtyCatalogCode { get; set; }
        public SpecialtyCatalog SpecialtyCatalog { get; set; }
        public string MedicalLicenseNumber { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public int YearsOfExperience { get; set; }
        public string ClinicAddress { get; set; }
        public string? Bio { get; set; }

        // --- Step 3: Availability & Practice ---
        public EnConsultationType ConsultationType { get; set; } // e.g., "In-Person", "Video", "Both"
        public decimal ConsultationFee { get; set; } = 0;

        // --- Step 4: Verification ---
        public string MedicalLicenseDocumentUrl { get; set; }
        public string NationalIdDocumentUrl { get; set; }
        public string? ProfileImageUrl { get; set; }
        public bool IsVerified { get; set; } = false;// This tracks if an admin has reviewed their uploaded documents

        //one to many ralations
        public List<AvailableTimeSlot> AvailableTimeSlots { get; set; } = new List<AvailableTimeSlot>();
        public List<string> AvailabeDays { get; set; } = new List<string>();
        public List<Facilitie> Facilities { get; set; } = new List<Facilitie>();
        public List<RadiologyOrder> RadiologyOrders { get; set; } = new List<RadiologyOrder>();
        public List<LabOrder> LabOrders { get; set; } = new List<LabOrder>();
        public List<Consent> Consents { get; set; }= new List<Consent>();
    }
    [Owned]
    public class AvailableTimeSlot
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
