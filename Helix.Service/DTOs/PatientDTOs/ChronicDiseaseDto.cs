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


    public class PatientLabDashboardDto
    {
        public int TotalTests { get; set; }
        public int PendingResults { get; set; }
        public int AbnormalResults { get; set; }

        // Nullable. If null, the Angular frontend simply hides the Alert widget.
        public LabAlertDto? ActiveAlert { get; set; }

        public List<LabTestItemDto> LabTests { get; set; } = new();
        public List<LabRecentActivityDto> RecentActivity { get; set; } = new();
    }

    public class LabAlertDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string TimeAgo { get; set; }
    }

    public class LabTestItemDto
    {
        public Guid Id { get; set; }
        public string TestName { get; set; }
        public DateTime Date { get; set; }
        public string RequestedBy { get; set; }

        // Maps to the UI pill colors: "Pending", "Completed", or "Abnormal"
        public string Status { get; set; }
    }

    public class LabRecentActivityDto
    {
        public string Title { get; set; }
        public string TimeAgo { get; set; }

        // Used by Angular to render the colored dot next to the activity
        public string StatusColor { get; set; }
    }


    public class LabTestDetailsDto
    {
        public Guid OrderId { get; set; }
        public string PanelName { get; set; } // e.g., "Complete Blood Count & Metabolic Panel"

        public LabPatientInfoDto PatientInfo { get; set; }
        public LabDoctorCommentDto DoctorComment { get; set; }
        public List<LabParameterResultDto> Results { get; set; } = new();
    }

    public class LabPatientInfoDto
    {
        public string PatientName { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public string DateOfBirth { get; set; } // e.g., "March 15, 1985 (41 years)"
        public string TestDate { get; set; }
        public string PatientIdDisplay { get; set; } // e.g., "PT-2026-4782"
    }

    public class LabDoctorCommentDto
    {
        public string DoctorName { get; set; }
        public string Specialty { get; set; }
        public string CommentDate { get; set; }
        public string OverallComment { get; set; }
        public List<string> Recommendations { get; set; } = new();
    }

    public class LabParameterResultDto
    {
        public string ParameterName { get; set; }
        public string ResultValue { get; set; }
        public string NormalRange { get; set; }
        public string Unit { get; set; }
        public string Status { get; set; }
    }
}