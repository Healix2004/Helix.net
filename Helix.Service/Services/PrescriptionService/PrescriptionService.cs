using Helix.Data.Entities;
using Helix.Data.Enums;
using Helix.Infrastructure.Context;
using Helix.Service.DTOs.DrugDTOs;
using Helix.Service.DTOs.LabOrderDTOs;
using Helix.Service.DTOs.PrescriptionDtos; // Ensure this matches your namespace
using Helix.Service.DTOs.RadiologyOrderDto;
using Helix.Service.Helper;
using Helix.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helix.Service.Services.PrescriptionService
{
    public class PrescriptionService(IUnitOfWork unitOfWork,ApplicationDbContext dbContext, IDrugDataService  drugDataService) : IPrescriptionService
    {

        public async Task<Guid> CreatePrescriptionAsync(Guid doctorId, PrescriptionPayloadDto dto)
        {
            var appointment = await unitOfWork.Repository<Appointment>().GetByIdAsync(dto.AppointmentId);
            var patientId = appointment.PatientId;
            var prescription = new Prescription
            {
                PatientId = patientId,
                DoctorId = doctorId,
                AppointmentId = dto.AppointmentId,
                CreatedAt = DateTime.Now
            };

            foreach (var itemDto in dto.Medications)
            {
                prescription.Items.Add(new PrescriptionItem
                {
                    MedicationCatalogRxcui = itemDto.Rxcui, // Handles both DTO variations
                    Dosage = itemDto.Dosage,
                    Frequency = itemDto.Frequency,
                    Duration = itemDto.Duration,
                    Status = EnPrescriptionItemStatus.Active
                });
            }

            await unitOfWork.Repository<Prescription>().AddAsync(prescription);

            if (dto.LabOrderCodes != null && dto.LabOrderCodes.Any())
            {
                foreach (var test in dto.LabOrderCodes)
                {
                    // Generate a unique 7-character order token for the barcode/QR
                    string qrToken = $"ORD-{Guid.NewGuid().ToString("N").Substring(0, 7).ToUpper()}";

                    var medicalConceptQuery = await unitOfWork.Repository<MedicalConcept>().FindAsQueryable(m => m.Code == test);
                    var newMedicalConcept = await medicalConceptQuery.FirstOrDefaultAsync();

                    if (newMedicalConcept == null) throw new KeyNotFoundException("Terminology code not found.");

                    var labOrder = new LabOrder
                    {
                        PatientId = patientId,
                        DoctorId = doctorId,
                        MedicalConceptId = newMedicalConcept.Id,
                        QrToken = qrToken,
                        Status = EnLabOrderStatus.Pending,
                        CreatedAt = DateTime.UtcNow
                    };

                    // FHIR PANEL EXPANSION LOGIC
                    var panelComponentsQuery = await unitOfWork.Repository<LoincPanelComponent>()
                        .FindAsQueryable(p => p.ParentLoincConceptId == newMedicalConcept.Id);

                    var childIds = await panelComponentsQuery.Select(p => p.ChildLoincConceptId).ToListAsync();

                    if (childIds.Any())
                    {
                        // It is a Panel: Create an empty slot for every child test
                        foreach (var childId in childIds)
                        {
                            labOrder.Results.Add(new LabTestResult
                            {
                                MedicalConceptId = childId,
                                PatientId = patientId,
                                Status = EnLabOrderStatus.Pending,
                                ResultDate = null
                            });
                        }
                    }
                    else
                    {
                        // It is a Single Test: Create one empty slot
                        labOrder.Results.Add(new LabTestResult
                        {
                            MedicalConceptId = newMedicalConcept.Id,
                            PatientId = patientId,
                            Status = EnLabOrderStatus.Pending,
                            ResultDate = null
                        });
                    }
                    prescription.LabOrders.Add(labOrder);
                }
            }

            if (dto.RadiologyOrderCodes != null && dto.RadiologyOrderCodes.Any())
            {
                foreach (var scan in dto.RadiologyOrderCodes)
                {
                    string qrToken = $"ORD-{Guid.NewGuid().ToString("N").Substring(0, 7).ToUpper()}";

                    var medicalConceptQuery = await unitOfWork.Repository<MedicalConcept>().FindAsQueryable(m => m.Code == scan);
                    var newMedicalConcept = await medicalConceptQuery.FirstOrDefaultAsync();

                    if (newMedicalConcept == null) throw new KeyNotFoundException("Terminology code not found.");

                    var radioOrder = new RadiologyOrder
                    {
                        PatientId = patientId,
                        DoctorId = doctorId,
                        MedicalConceptId = newMedicalConcept.Id,
                        QrToken = qrToken,
                        Status = EnRadiologyOrderStatus.Pending,
                        CreatedAt = DateTime.UtcNow
                    };
                    prescription.RadiologyOrders.Add(radioOrder);
                }
            }
            appointment.Status = EnAppointmentStatus.Fulfilled;
            await unitOfWork.Repository<Appointment>().UpdateAsync(appointment);

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

        public async Task<PrescriptionSafetyResultDto> CheckCompletePrescriptionSafetyAsync(PrescriptionSafetyCheckDto request)
        {
            var result = new PrescriptionSafetyResultDto { IsSafe = true };

            if (request.NewMedicationIds == null || !request.NewMedicationIds.Any())
                return result;

            // 1. Fetch Patient's CURRENT Active Medications from the Database
            var patient = await dbContext.Patients
                .Include(p => p.Medications) // Ensure this is explicitly included if Lazy Loading is off
                .FirstOrDefaultAsync(p => p.Id == request.PatientId);

            if (patient == null || patient.Medications == null)
                return result;

            var activePrescriptionItems = patient.Medications.Where(med =>
                !med.EndDate.HasValue ||
                med.EndDate.Value.ToDateTime(System.TimeOnly.MinValue) > DateTime.UtcNow);

            // These are Rxcui strings
            var currentMedIds = activePrescriptionItems.Select(pi => pi.medicationCatalogRxcui).ToList();

            // 2. Bulk Fetch AiModelNames from DbContext (MUST do this before multi-threading!)
            var allRxcuis = currentMedIds.Concat(request.NewMedicationIds).Distinct().ToList();

            // Create a dictionary mapping the Rxcui (e.g., "1191") to the AiModelName (e.g., "Aspirin")
            var rxCuiToAiNameMap = await dbContext.MedicationCatalogs
                .Where(m => allRxcuis.Contains(m.Rxcui) && m.AiModelName != null)
                .ToDictionaryAsync(m => m.Rxcui, m => m.AiModelName);

            // 3. Generate Pairs to Check using the Rxcuis
            var pairsToCheck = new HashSet<(string RxcuiA, string RxcuiB)>();

            // Pair New vs Current
            foreach (var newMed in request.NewMedicationIds)
            {
                foreach (var currentMed in currentMedIds)
                {
                    if (newMed != currentMed)
                        pairsToCheck.Add((newMed, currentMed));
                }
            }

            // Pair New vs New
            for (int i = 0; i < request.NewMedicationIds.Count; i++)
            {
                for (int j = i + 1; j < request.NewMedicationIds.Count; j++)
                {
                    pairsToCheck.Add((request.NewMedicationIds[i], request.NewMedicationIds[j]));
                }
            }

            // 4. Fire Concurrent AI Checks
            var checkTasks = new List<Task<InteractionResponseDTO?>>();

            foreach (var pair in pairsToCheck)
            {
                // Translate Rxcui to AiModelName safely
                if (!rxCuiToAiNameMap.TryGetValue(pair.RxcuiA, out var aiNameA) ||
                    !rxCuiToAiNameMap.TryGetValue(pair.RxcuiB, out var aiNameB))
                {
                    continue; // Skip if one of the drugs isn't mapped to an AI name
                }

                // Now translate the AiModelName to your Cache Integer ID
                var id1 = await drugDataService.GetIdAsync(aiNameA);
                var id2 = await drugDataService.GetIdAsync(aiNameB);

                if (!id1.HasValue || !id2.HasValue)
                    continue;

                var dto = new InteractionRequestDTO { IdDrug1 = id1.Value, IdDrug2 = id2.Value };
                checkTasks.Add(drugDataService.CheckDrugInteractionAsync(dto));
            }

            // Await all API calls simultaneously 
            var aiResponses = await Task.WhenAll(checkTasks);

            // 5. Filter Results
            foreach (var aiResponse in aiResponses)
            {
                if (aiResponse != null && aiResponse.IsInteraction && aiResponse.Confidence > 50.0)
                {
                    result.DangerousInteractions.Add(aiResponse);
                    result.IsSafe = false;
                }
            }

            return result;
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