using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Helix.Service.DTOs.Pharmacy
{
    public class RegisterPharmacyDto
    {
        [Required(ErrorMessage = "National ID is required.")]
        public string NationalId { get; set; }

        [Required(ErrorMessage = "Pharmacy Name is required.")]
        public string PharmacyName { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "License Number is required.")]
        public string LicenseNumber { get; set; }

        // --- File Uploads ---

        // Optional profile image
        public IFormFile? ProfileImageFile { get; set; }

        [Required(ErrorMessage = "The primary license document is required.")]
        public IFormFile PrimaryLicenseFile { get; set; }

        [Required(ErrorMessage = "The National ID document is required.")]
        public IFormFile NationalIdFile { get; set; }
    }

    public class PharmacyDashboardDto
    {
        // Top Metric Cards
        public int PrescriptionsDispensedToday { get; set; }
        public string DispensedTrend { get; set; } // e.g., "+15%"

        public int PendingOrders { get; set; }
        public string PendingTrend { get; set; } // e.g., "-33%"

        public int CriticalAlerts { get; set; }
        public string AlertsTrend { get; set; } // e.g., "-25%"

        public decimal RevenueToday { get; set; }
        public string RevenueTrend { get; set; } // e.g., "+8%"

        // Chart Data
        public List<PrescriptionTrendDto> PrescriptionTrends { get; set; } = new();

        // Tables & Sidebar Lists
        public List<TodaysPrescriptionDto> TodaysPrescriptions { get; set; } = new();
        public List<StockAlertDto> StockAlerts { get; set; } = new();

        // Dispensing Accuracy Widget
        public double DispensingAccuracy { get; set; }
        public string AccuracyMessage { get; set; }
    }

    public class PrescriptionTrendDto
    {
        public string Month { get; set; } // e.g., "Jan", "Feb"
        public int Dispensed { get; set; }
        public int Pending { get; set; }
    }

    public class TodaysPrescriptionDto
    {
        public string PatientInitials { get; set; } // e.g., "SA" for Sarah Al-Mansouri
        public string PatientName { get; set; }
        public string RxId { get; set; } // e.g., "RX-2026-77421"
        public string Department { get; set; }
        public string Time { get; set; }
        public string Status { get; set; } // "Active", "Pending", "Dispensed"
    }

    public class StockAlertDto
    {
        public string MedicationName { get; set; }
        public string AlertMessage { get; set; } // e.g., "Critical restock", "low stock - reorder"
        public bool IsCritical { get; set; } // Used to toggle the red vs yellow warning icon
    }


    public class PharmacyPrescriptionDetailsDto
    {
        public Guid PrescriptionId { get; set; }
        public string Status { get; set; } // e.g., "Active Patient", "Pending"

        // --- Patient Info Header ---
        public string PatientName { get; set; }
        public string PatientIdDisplay { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Insurance { get; set; }

        // --- Alerts ---
        public List<string> Allergies { get; set; } = new();

        // --- Prescription Body ---
        public string Diagnosis { get; set; }
        public List<PharmacyMedicationItemDto> Medications { get; set; } = new();

        // --- Doctor Info ---
        public string DoctorName { get; set; }
        public string DoctorIdDisplay { get; set; }
        public string DoctorSpecialty { get; set; }
        public string IssueDate { get; set; }
    }

    public class PharmacyMedicationItemDto
    {
        public string DrugName { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
        public string Instructions { get; set; }
    }

    // DTO for the "Flag for Review" payload
    public class FlagPrescriptionDto
    {
        public string Reason { get; set; } // e.g., "Potential drug interaction with Penicillin"
    }
}
