using Helix.Data.Entities;
using Helix.Data.Enums;
using Helix.Infrastructure.Context;
using Helix.Service.DTOs.MedicalHistoryDtos;
using Helix.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helix.Service.Services.MedicalHistory
{
    public class MedicalHistoryService : IMedicalHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MedicalHistoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TimelineEventDto>> GetPatientTimelineAsync(Guid patientId)
        {
            // 1. Await each query sequentially to protect the DbContext thread
            var appointments = await FetchAppointmentsAsync(patientId);
            var labs = await FetchLabOrdersAsync(patientId);
            var radiologyScans = await FetchRadiologyScansAsync(patientId);

            // 2. Pre-allocate list size for better memory performance
            var timelineEvents = new List<TimelineEventDto>(
                appointments.Count + labs.Count + radiologyScans.Count);

            // 3. Combine the results
            timelineEvents.AddRange(appointments);
            timelineEvents.AddRange(labs);
            timelineEvents.AddRange(radiologyScans);

            // 4. Sort chronologically (Newest first)
            return timelineEvents.OrderByDescending(e => e.Date).ToList();
        }

        // --- Private Query & Mapping Helpers ---

        private async Task<List<TimelineEventDto>> FetchAppointmentsAsync(Guid patientId)
        {
            var query = await _unitOfWork.Repository<Appointment>()
                .FindAsQueryable(a => a.PatientId == patientId && a.Status == EnAppointmentStatus.Fulfilled);

            var appointments = await query.Include(a => a.Doctor).ToListAsync();

            return appointments.Select(a => new TimelineEventDto
            {
                Id = a.Id,
                EventType = "Appointment",
                Title = a.AppointmentType ?? "General Examination",
                Date = a.StartTime,
                Description = "Routine checkup and screening.",
                // Fixed: Prevents outputting "Dr. " if the Doctor object is null
                Provider = a.Doctor != null ? $"Dr. {a.Doctor.FullName}" : "Unknown Provider",
                Details = new List<TimelineDetailDto>
                {
                    new TimelineDetailDto { Label = "Vital Signs:", Value = "Not recorded" },
                    new TimelineDetailDto { Label = "Recommendations:", Value = "None" }
                }
            }).ToList();
        }

        private async Task<List<TimelineEventDto>> FetchLabOrdersAsync(Guid patientId)
        {
            var query = await _unitOfWork.Repository<LabOrder>()
                .FindAsQueryable(l => l.PatientId == patientId && l.Status == EnLabOrderStatus.Completed);

            var labOrders = await query
                .Include(l => l.Doctor)
                .Include(l => l.MedicalConcept)
                .Include(l => l.Results)
                    .ThenInclude(r => r.MedicalConcept)
                .ToListAsync();

            return labOrders.Select(l => new TimelineEventDto
            {
                Id = l.Id,
                EventType = "Lab",
                Title = "Laboratory Results",
                Date = l.CreatedAt,
                Description = $"Diagnostic blood work for {l.MedicalConcept?.Display ?? l.MedicalConcept?.Code}.",
                Provider = l.Doctor != null ? $"Ordered by Dr. {l.Doctor.FullName}" : "Unknown Provider",
                Details = new[]
                {
                    new TimelineDetailDto { Label = "Panel:", Value = l.MedicalConcept?.Display ?? "Unknown" }
                }
                .Concat(
                    l.Results?.Select(res => new TimelineDetailDto
                    {
                        Label = $"{res.MedicalConcept?.Display ?? "Result"}:",
                        Value = FormatLabResultValue(res)
                    }) ?? Array.Empty<TimelineDetailDto>()
                ).ToList()
            }).ToList();
        }

        private async Task<List<TimelineEventDto>> FetchRadiologyScansAsync(Guid patientId)
        {
            var query = await _unitOfWork.Repository<RadiologyOrder>()
                .FindAsQueryable(r => r.PatientId == patientId && r.Status == EnRadiologyOrderStatus.Completed);

            var radiologyScans = await query
                .Include(r => r.Doctor)
                .Include(r => r.MedicalConcept)
                .Include(r => r.Report)
                .ToListAsync();

            return radiologyScans.Select(r => new TimelineEventDto
            {
                Id = r.Id,
                EventType = "Radiology",
                Title = "Radiology Scan",
                Date = r.CreatedAt,
                Description = $"Imaging procedure: {r.MedicalConcept?.Code}.",
                Provider = r.Doctor != null ? $"Ordered by Dr. {r.Doctor.FullName}" : "Unknown Provider",
                Details = r.Report != null
                    ? new List<TimelineDetailDto>
                    {
                        new TimelineDetailDto { Label = "Scan Type:", Value = r.MedicalConcept?.Display ?? "Unknown" },
                        new TimelineDetailDto { Label = "Conclusion:", Value = r.Report.Conclusion },
                        new TimelineDetailDto { Label = "Images:", Value = r.Report.ImageUrls != null && r.Report.ImageUrls.Any() ? $"{r.Report.ImageUrls.Count} attached" : "None" },
                        new TimelineDetailDto { Label = "Radiologist:", Value = r.Report.ExternalRadiologistName ?? "Internal Staff" }
                    }
                    : new List<TimelineDetailDto>
                    {
                        new TimelineDetailDto { Label = "Scan Type:", Value = r.MedicalConcept?.Display ?? "Unknown" },
                        new TimelineDetailDto { Label = "Status:", Value = "Report processing" }
                    }
            }).ToList();
        }

        // --- Helper Methods ---

        private string FormatLabResultValue(LabTestResult res)
        {
            var val = res.NumericValue.HasValue
                ? res.NumericValue.Value.ToString("G")
                : (res.StringValue ?? "No value recorded");

            var unit = !string.IsNullOrWhiteSpace(res.Unit) ? $" {res.Unit}" : "";
            var range = !string.IsNullOrWhiteSpace(res.ReferenceRange) ? $" (Normal: {res.ReferenceRange})" : "";
            var flag = !string.IsNullOrWhiteSpace(res.InterpretationFlag) ? $" [{res.InterpretationFlag.ToUpper()}]" : "";

            return $"{val}{unit}{range}{flag}".Trim();
        }
    }
}