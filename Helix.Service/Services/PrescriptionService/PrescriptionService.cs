using Helix.Data.Entities;
using Helix.Data.Enums;
using Helix.Service.DTOs.PrescriptionDtos; // Ensure this matches your namespace
using Helix.Service.Helper;
using Helix.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helix.Service.Services.PrescriptionService
{
    public class PrescriptionService(IUnitOfWork unitOfWork) : IPrescriptionService
    {
        public async Task<Guid> CreatePrescriptionAsync(Guid doctorId, CreatePrescriptionDto dto)
        {
            var prescription = new Prescription
            {
                PatientId = dto.PatientId,
                DoctorId = doctorId,
                AppointmentId = dto.AppointmentId,
                DoctorNotes = dto.DoctorNotes,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var itemDto in dto.Medications)
            {
                prescription.Items.Add(new PrescriptionItem
                {
                    MedicationCatalogRxcui = itemDto.TerminologyRxcui,
                    Dosage = itemDto.Dosage,
                    Frequency = itemDto.Frequency,
                    Duration = itemDto.Duration,
                    Status = EnPrescriptionItemStatus.Active
                });
            }

            await unitOfWork.Repository<Prescription>().AddAsync(prescription);
            await unitOfWork.CompleteAsync();

            return prescription.Id;
        }

        public async Task<PatientClinicalSummaryDto> GetPatientClinicalSummaryAsync(Guid patientId)
        {
            var patient = await unitOfWork.Repository<Patient>().GetByIdAsync(patientId);
            if (patient == null) return null;

            var activeMeds = await (await unitOfWork.Repository<PrescriptionItem>()
                .FindAsQueryable(i => i.Prescription.PatientId == patientId && i.Status == EnPrescriptionItemStatus.Active))
                .Include(i => i.MedicationCatalog)
                .Include(i => i.Prescription)
                .ToListAsync();

            return new PatientClinicalSummaryDto
            {
                PatientId = patient.Id,
                PatientName = patient.FullName,
                Initials = GetInitials(patient.FullName),
                DisplayId = $"{patient.NationalId.Substring(0, 5)}-A", // Assuming logic for ID display
                Age = CalculateAge(patient.NationalId.ParseEgyptianId().dateOfBirth),
                Gender = patient.NationalId.ParseEgyptianId().gender.ToString(),
                // Placeholder lists: replace these with real database calls to Allergy/Condition tables
                Allergies = new List<string> { "Penicillin", "Sulfa drugs" },
                ChronicConditions = new List<string> { "Hypertension (controlled)", "Type 2 Diabetes (controlled)" },
                CurrentMedications = activeMeds.Select(m => new ActiveMedicationDto
                {
                    PrescriptionItemId = m.Id,
                    MedicationName = m.MedicationCatalog.DrugName,
                    Instructions = $"{m.Dosage} - {m.Frequency}",
                    PrescribedDate = m.Prescription.CreatedAt,
                    Status = m.Status.ToString()
                }).ToList()
            };
        }

        public async Task<bool> UpdatePrescriptionItemStatusAsync(Guid itemId, EnPrescriptionItemStatus newStatus)
        {
            var item = await unitOfWork.Repository<PrescriptionItem>().GetByIdAsync(itemId);
            if (item == null) return false;

            item.Status = newStatus;
            await unitOfWork.Repository<PrescriptionItem>().UpdateAsync(item);
            return await unitOfWork.CompleteAsync() > 0;
        }

        // --- Helper Methods ---
        private string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "N/A";
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 1
                ? $"{parts[0][0]}{parts[^1][0]}".ToUpper()
                : parts[0][0].ToString().ToUpper();
        }

        private int CalculateAge(DateOnly dob)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - dob.Year;

            if (dob > today.AddYears(-age))
            {
                age--;
            }
            return age;
        }
    }
}