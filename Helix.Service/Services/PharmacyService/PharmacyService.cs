using Helix.Data.Entities;
using Helix.Data.Enums;
using Helix.Service.DTOs.Pharmacy;
using Helix.Service.Helper;
using Helix.Service.Interfaces;
using Helix.Service.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Service.Services.PharmacyService
{
    public class PharmacyService(IUnitOfWork unitOfWork, IFileService fileService) : IPharmacyService
    {
        public async Task<Pharmacy> RegisterPharmacyAsync(RegisterPharmacyDto dto, Guid appUserId)
        {
            // 1. Ensure the user hasn't already registered a pharmacy
            var existingPharmacy = await (await unitOfWork.Repository<Pharmacy>()
                .FindAsQueryable(p => p.AppUserId == appUserId))
                .FirstOrDefaultAsync();

            if (existingPharmacy != null)
            {
                throw new InvalidOperationException("A pharmacy profile is already associated with this account.");
            }

            // 2. Handle File Uploads
            // These will upload the documents and return the string URLs to store in the database
            string primaryLicenseUrl = await fileService.UploadFileAsync(dto.PrimaryLicenseFile, "pharmacy/licenses");
            string nationalIdUrl = await fileService.UploadFileAsync(dto.NationalIdFile, "pharmacy/national-ids");

            string profileImageUrl = null;
            if (dto.ProfileImageFile != null)
            {
                profileImageUrl = await fileService.UploadFileAsync(dto.ProfileImageFile);
            }

            // 3. Map DTO to Entity
            var pharmacy = new Pharmacy
            {
                AppUserId = appUserId,
                NationalId = dto.NationalId,
                PharmacyName = dto.PharmacyName,
                Address = dto.Address,
                LicenseNumber = dto.LicenseNumber,

                // Keep as false so Helix admins can review the uploaded documents
                IsVerified = false,

                PrimaryLicenseUrl = primaryLicenseUrl,
                NationalIdUrl = nationalIdUrl,
                ProfileImageUrl = profileImageUrl
            };

            // 4. Save to Database
            await unitOfWork.Repository<Pharmacy>().AddAsync(pharmacy);

            // Assuming your UnitOfWork has a CompleteAsync or SaveChangesAsync method
            await unitOfWork.CompleteAsync();

            return pharmacy;
        }

        public async Task<PharmacyDashboardDto> GetDashboardAsync(Guid appUserId)
        {
            // 1. Find the Pharmacy belonging to the logged-in Pharmacist
            var pharmacy = await (await unitOfWork.Repository<Pharmacy>()
                .FindAsQueryable(p => p.AppUserId == appUserId))
                .FirstOrDefaultAsync();

            if (pharmacy == null) return null; // Handled as 404 in controller

            var today = DateTime.UtcNow.Date;
            var dashboard = new PharmacyDashboardDto();

            // 2. Fetch Today's Prescriptions routed to this pharmacy
            // Note: Adjust the entity mapping based on how Prescriptions tie to Pharmacies in your DB
            var todaysOrders = await (await unitOfWork.Repository<Prescription>()
                .FindAsQueryable(p => p.PharmacyId == pharmacy.Id && p.CreatedAt >= today))
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                    .ThenInclude(d => d.SpecialtyCatalog) // For the Department column
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            // 3. Calculate Top Metric Cards
            dashboard.PrescriptionsDispensedToday = todaysOrders.Count(p => p.Status == EnPrescriptionStatus.Dispensed);
            dashboard.PendingOrders = todaysOrders.Count(p => p.Status == EnPrescriptionStatus.Pending);
            dashboard.RevenueToday = 142.00m;
            // Mocking the trend percentages (In production, query yesterday's data to calculate the delta)
            dashboard.DispensedTrend = "+15%";
            dashboard.PendingTrend = "-33%";
            dashboard.RevenueTrend = "+8%";

            // 4. Map Today's Prescriptions Table
            dashboard.TodaysPrescriptions = todaysOrders.Take(5).Select(p => new TodaysPrescriptionDto
            {
                PatientName = p.Patient?.FullName ?? "Unknown Patient",
                PatientInitials = GetInitials(p.Patient?.FullName),
                RxId = $"RX-{today.Year}-{p.Id.ToString().Substring(0, 5).ToUpper()}",
                Department = p.Doctor?.SpecialtyCatalog?.DisplayName ?? "General",
                Time = p.CreatedAt.ToString("hh:mm tt"),
                Status = p.Status.ToString() // E.g., "Pending", "Dispensed", "Active"
            }).ToList();

            // 5. Mock Inventory for Stock Alerts (Until the Inventory module is built)
            dashboard.StockAlerts = new List<StockAlertDto>
            {
                new StockAlertDto
                {
                    MedicationName = "Metformin 500mg",
                    IsCritical = true,
                    AlertMessage = "Critical restock"
                },
                new StockAlertDto
                {
                    MedicationName = "Lisinopril 10mg",
                    IsCritical = false,
                    AlertMessage = "low stock - reorder"
                }
            };

            dashboard.CriticalAlerts = dashboard.StockAlerts.Count(a => a.IsCritical);
            dashboard.AlertsTrend = "-25%";

            // 6. Generate Chart Data (Aggregated by month for the current year)
            // For optimal performance, this should ideally be a raw SQL GROUP BY query or cached.
            dashboard.PrescriptionTrends = GenerateMockChartData();

            // 7. Accuracy Metrics
            dashboard.DispensingAccuracy = 98.7;
            dashboard.AccuracyMessage = "Excellent accuracy this month";

            return dashboard;
        }

        public async Task<PharmacyPrescriptionDetailsDto> GetPrescriptionDetailsForPharmacyAsync(Guid prescriptionId, Guid appUserId)
        {
            // 1. Verify the pharmacist's pharmacy
            var pharmacy =await (await unitOfWork.Repository<Pharmacy>()
                .FindAsQueryable(p => p.AppUserId == appUserId))
                .FirstOrDefaultAsync();

            if (pharmacy == null) return null;

            // 2. Fetch the specific prescription
            // Ensure we also include the Patient, their AppUser (for email/phone), and the Doctor
            var prescription = await( await unitOfWork.Repository<Prescription>()
                .FindAsQueryable(p => p.Id == prescriptionId && p.PharmacyId == pharmacy.Id))
                .Include(p => p.Patient)
                    .ThenInclude(pat => pat.AppUser)
                .Include(p => p.Doctor)
                    .ThenInclude(doc => doc.SpecialtyCatalog)
                .Include(p => p.Items)
                    .ThenInclude(item => item.MedicationCatalog)
                .FirstOrDefaultAsync();

            if (prescription == null) return null;

            // 3. Calculate Age safely
            int age = 0;
            if (prescription.Patient != null && !string.IsNullOrWhiteSpace(prescription.Patient.NationalId))
            {
                var dob = prescription.Patient.NationalId.ParseEgyptianId().dateOfBirth;
                var today = DateOnly.FromDateTime(DateTime.Today);
                age = today.Year - dob.Year;
                if (dob.AddYears(age) > today) age--;
            }

            // 4. Map the data
            return new PharmacyPrescriptionDetailsDto
            {
                PrescriptionId = prescription.Id,
                Status = "Active Patient", // You can map this based on patient or prescription status

                PatientName = prescription.Patient?.FullName ?? "Unknown",
                PatientIdDisplay = prescription.Patient != null ? $"{prescription.Patient.Id.ToString().Substring(0, 5).ToUpper()}-A" : "UNKNOWN",
                Age = age,
                Gender = prescription.Patient?.NationalId.ParseEgyptianId().gender.ToString() ?? "Unknown",
                Phone = prescription.Patient?.AppUser?.PhoneNumber ?? "No Phone",
                Email = prescription.Patient?.AppUser?.Email ?? "No Email",
                Insurance = "Blue Shield Premium", // Mocked: Map this to your patient's insurance entity if you have one

                // Mocking allergies until an Allergy entity is added to the patient profile
                Allergies = new List<string> { "Penicillin", "Sulfonamides" },

                Diagnosis = prescription.DoctorNotes ?? "Type 2 Diabetes Mellitus with Hypertension", // Map to your medical concept

                Medications = prescription.Items?.Select(m => new PharmacyMedicationItemDto
                {
                    DrugName = m.MedicationCatalog.DrugName,
                    Dosage = m.Dosage,
                    Frequency = m.Frequency,
                    Duration = m.Duration,
                }).ToList() ?? new List<PharmacyMedicationItemDto>(),

                DoctorName = prescription.Doctor != null ? $"Dr. {prescription.Doctor.FullName}" : "Unknown Doctor",
                DoctorIdDisplay = prescription.Doctor != null ? $"MED-SA-{prescription.Doctor.Id.ToString().Substring(0, 5).ToUpper()}" : "UNKNOWN",
                DoctorSpecialty = prescription.Doctor?.SpecialtyCatalog?.DisplayName ?? "Internal Medicine",
                IssueDate = prescription.CreatedAt.ToString("dd MMM yyyy")
            };
        }

        public async Task<bool> DispensePrescriptionAsync(Guid prescriptionId, Guid appUserId)
        {
            var pharmacy = await (await unitOfWork.Repository<Pharmacy>().FindAsQueryable(p => p.AppUserId == appUserId)).FirstOrDefaultAsync();
            if (pharmacy == null) return false;

            var prescription = await( await unitOfWork.Repository<Prescription>()
                .FindAsQueryable(p => p.Id == prescriptionId && p.PharmacyId == pharmacy.Id))
                .FirstOrDefaultAsync();

            if (prescription == null || prescription.Status == EnPrescriptionStatus.Dispensed) return false;

            // Update status
            prescription.Status = EnPrescriptionStatus.Dispensed;

            // In a real system, you would also deduct the medications from the InventoryItem entity here!

            await unitOfWork.Repository<Prescription>().UpdateAsync(prescription);
            await unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> FlagPrescriptionAsync(Guid prescriptionId, FlagPrescriptionDto dto, Guid appUserId)
        {
            var pharmacy = await (await unitOfWork.Repository<Pharmacy>().FindAsQueryable(p => p.AppUserId == appUserId)).FirstOrDefaultAsync();
            if (pharmacy == null) return false;

            var prescription = await (await unitOfWork.Repository<Prescription>()
                .FindAsQueryable(p => p.Id == prescriptionId && p.PharmacyId == pharmacy.Id))
                .FirstOrDefaultAsync();

            if (prescription == null) return false;

            // Revert status to pending or a specific flagged status
            // Assuming EnPrescriptionStatus has a Flagged or Pending state
            prescription.Status = EnPrescriptionStatus.Pending;

            await unitOfWork.Repository<Prescription>().UpdateAsync(prescription);
            await unitOfWork.CompleteAsync();
            return true;
        }

        // --- Helper Methods ---
        private string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "UN";
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();
            return $"{parts[0][0]}{parts[1][0]}".ToUpper();
        }

        private List<PrescriptionTrendDto> GenerateMockChartData()
        {
            // Replace with actual database grouping logic querying the last 5 months
            return new List<PrescriptionTrendDto>
            {
                new PrescriptionTrendDto { Month = "Jan", Dispensed = 85, Pending = 40 },
                new PrescriptionTrendDto { Month = "Feb", Dispensed = 95, Pending = 35 },
                new PrescriptionTrendDto { Month = "Mar", Dispensed = 120, Pending = 45 },
                new PrescriptionTrendDto { Month = "Apr", Dispensed = 110, Pending = 40 },
                new PrescriptionTrendDto { Month = "May", Dispensed = 130, Pending = 25 }
            };
        }
    }
}
