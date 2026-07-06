using Helix.Data.Entities;
using Helix.Data.Enums;
using Helix.Service.DTOs.PatientDTOs;
using Helix.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Helix.Service.Services.PatientService
{
    public class PatientDashboardService(IUnitOfWork unitOfWork) : IPatientDashboardService
    {
        public async Task<PatientPortalDashboardDto> GetDashboardDataAsync(Guid patientId)
        {
            var now = DateTime.UtcNow;
            var dashboard = new PatientPortalDashboardDto();

            // 1. Calculate Top Metric Cards
            var labsCount = await (await unitOfWork.Repository<LabOrder>()
                .FindAsQueryable(l => l.PatientId == patientId && l.Status == EnLabOrderStatus.Completed)).CountAsync();

            var radiologyCount = await (await unitOfWork.Repository<RadiologyOrder>()
                .FindAsQueryable(r => r.PatientId == patientId && r.Status == EnRadiologyOrderStatus.Completed)).CountAsync();
            dashboard.TotalRecords = labsCount + radiologyCount; // Or include uploaded documents if applicable

            dashboard.ActivePrescriptions = await (await unitOfWork.Repository<Prescription>()
                .FindAsQueryable(p => p.PatientId == patientId )).CountAsync();

            dashboard.UpcomingAppointments = await (await unitOfWork.Repository<Appointment>()
                .FindAsQueryable(a => a.PatientId == patientId && a.StartTime > now && a.Status == EnAppointmentStatus.Booked)).CountAsync();
            // 2. Fetch Recent Activity (Mixed feed)
            var activities = new List<RecentActivityItemDto>();

            // Get latest 2 labs
            var recentLabs = await(await unitOfWork.Repository<LabOrder>()
                .FindAsQueryable(l => l.PatientId == patientId && l.Status == EnLabOrderStatus.Completed))
                .Include(l => l.MedicalConcept)
                .OrderByDescending(l => l.CreatedAt)
                .Take(2).ToListAsync();

            activities.AddRange(recentLabs.Select(l => new RecentActivityItemDto
            {
                ActivityType = "Lab",
                Title = "Lab Results Available",
                Description = $"Blood test results for {l.MedicalConcept?.Display ?? "General Panel"}",
                Date = l.CreatedAt,
                TimeAgo = GetTimeAgo(l.CreatedAt)
            }));

            // Get latest 2 upcoming appointments
            var upcomingAppts = await (await unitOfWork.Repository<Appointment>()
                .FindAsQueryable(a => a.PatientId == patientId && a.StartTime > now))
                .Include(a => a.Doctor)
                .OrderBy(a => a.StartTime)
                .Take(2).ToListAsync();

            activities.AddRange(upcomingAppts.Select(a => new RecentActivityItemDto
            {
                ActivityType = "Appointment",
                Title = "Appointment Confirmed",
                Description = $"Dr. {a.Doctor?.FullName} - {a.StartTime:MMM dd, yyyy}",
                Date = a.StartTime, // Note: For upcoming, TimeAgo logic might need adjusting or hiding
                TimeAgo = $"In {(a.StartTime - now).Days} days"
            }));

            //Get latest 2 radiology 
            var recentRadiology = await (await unitOfWork.Repository<RadiologyOrder>()
                .FindAsQueryable(r => r.PatientId == patientId && r.Status == EnRadiologyOrderStatus.Completed))
                .Include(r => r.MedicalConcept)
                .OrderByDescending(r => r.CreatedAt)
                .Take(2).ToListAsync();
            
            activities.AddRange(recentRadiology.Select(r => new RecentActivityItemDto
            {
                ActivityType = "Radiology",
                Title = "Radiology Results Available",
                Description = $"Radiology results for {r.MedicalConcept?.Display ?? "General Scan"}",
                Date = r.CreatedAt,
                TimeAgo = GetTimeAgo(r.CreatedAt)
            }));

            // Get latest 2 prescriptions
            var recentPrescriptions = await (await unitOfWork.Repository<Prescription>()
                .FindAsQueryable(p => p.PatientId == patientId))
                .Include(p => p.Items).ThenInclude(i=> i.MedicationCatalog)
                .OrderByDescending(p => p.CreatedAt)
                .Take(2).ToListAsync();
            
            activities.AddRange(recentPrescriptions.Select(p => new RecentActivityItemDto
            {
                ActivityType = "Prescription",
                Title = "Prescription Updated",
                Description = $"Medication: {p.Items?.FirstOrDefault()?.MedicationCatalog?.DrugName ?? "General Medication"}",
                Date = p.CreatedAt,
                TimeAgo = GetTimeAgo(p.CreatedAt)
            }));

            // Sort mixed activities by date and take the top 4 for the UI
            dashboard.RecentActivity = activities.OrderByDescending(a => a.Date).Take(4).ToList();

            // 3. Mock Health Score & Tips (Can be dynamic later based on actual patient vitals)
            dashboard.HealthScore = 85;
            dashboard.HealthScoreMessage = "Your health metrics are looking good!";

            dashboard.HealthTips = new List<HealthTipDto>
            {
                new HealthTipDto { IconType = "Heart", Title = "Stay Hydrated", Description = "Drink at least 8 glasses of water daily" },
                new HealthTipDto { IconType = "Activity", Title = "Daily Exercise", Description = "30 minutes of moderate activity" },
                new HealthTipDto { IconType = "Drop", Title = "Monitor Blood Sugar", Description = "Check levels before meals" }
            };

            return dashboard;
        }

        // --- Helper Method ---
        private string GetTimeAgo(DateTime date)
        {
            var timeSpan = DateTime.UtcNow - date;

            if (timeSpan.TotalMinutes < 60) return timeSpan.TotalMinutes < 2 ? "Just now" : $"{(int)timeSpan.TotalMinutes} minutes ago";
            if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours} hours ago";
            if (timeSpan.TotalDays < 30) return $"{(int)timeSpan.TotalDays} days ago";

            return $"{(int)(timeSpan.TotalDays / 30)} months ago";
        }
    }
}