using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Service.DTOs.RadiologyReportDtos
{
    public class RadiologyReportDto
    {
        public Guid Id { get; set; }
        public Guid RadiologyOrderId { get; set; }

        // --- Context Data (Fetched via .Include() from the Order) ---
        public string PatientName { get; set; } = string.Empty;
        public string RequestingDoctorName { get; set; } = string.Empty;
        public string TerminologyDisplay { get; set; } = string.Empty; // e.g., "MR Brain WO contrast"

        // --- Core Report Data ---
        public string? ExternalRadiologistName { get; set; }
        public string Findings { get; set; } = string.Empty;
        public string Conclusion { get; set; } = string.Empty;
        public DateTime ReportDate { get; set; }

        // --- Imaging ---
        // A clean list of direct URLs (Azure/AWS/Local) for your Flutter/Angular app to render
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
