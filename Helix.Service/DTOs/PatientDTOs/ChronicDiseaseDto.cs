namespace Helix.Service.DTOs.PatientDTOs
{
    public class ChronicDiseaseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime DiagnosisDate { get; set; }
    }
    public class PatientPortalDashboardDto
    {
        // Top Cards
        public int TotalRecords { get; set; }
        public int ActivePrescriptions { get; set; }
        public int UpcomingAppointments { get; set; }

        // Health Score Widget
        public int HealthScore { get; set; }
        public string HealthScoreMessage { get; set; }

        // Lists
        public List<RecentActivityItemDto> RecentActivity { get; set; } = new();
        public List<HealthTipDto> HealthTips { get; set; } = new();
    }

    public class RecentActivityItemDto
    {
        public string ActivityType { get; set; } // e.g., "Lab", "Prescription", "Appointment"
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string TimeAgo { get; set; } // e.g., "2 hours ago"
    }

    public class HealthTipDto
    {
        public string IconType { get; set; } // e.g., "Heart", "Activity", "Drop"
        public string Title { get; set; }
        public string Description { get; set; }
    }
}