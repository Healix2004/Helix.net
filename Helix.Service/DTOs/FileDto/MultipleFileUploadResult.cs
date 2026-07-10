namespace Helix.Service.DTOs.FileDto
{
    public class MultipleFileUploadResult
    {
        public bool Success { get; set; }
        public List<string> FilePaths { get; set; } = new();
        public string ErrorMessage { get; set; }
    }
    // The main wrapper for the entire page
    public class DocumentDashboardDto
    {
        public DocumentSummaryDto Summary { get; set; }
        public StorageUsageDto Storage { get; set; }
        public List<RecentUploadDto> RecentUploads { get; set; }
        public List<DocumentItemDto> Documents { get; set; }
    }

    public class DocumentSummaryDto
    {
        public int TotalFiles { get; set; }
        public int RecentUploadsCount { get; set; } // Uploaded in the last 7 days
        public int SharedFilesCount { get; set; }
    }

    public class StorageUsageDto
    {
        public double UsedSpaceGb { get; set; }
        public double TotalSpaceGb { get; set; }
        public double ReportsGb { get; set; }
        public double ScansGb { get; set; }
        public double OtherGb { get; set; }
    }

    public class RecentUploadDto
    {
        public string FileName { get; set; }
        public string TimeAgo { get; set; } // e.g., "2 hours ago"
    }

    public class DocumentItemDto
    {
        public string Id { get; set; }
        public string FileName { get; set; }
        public string Type { get; set; } // Report, Scan, Prescription, Form
        public string UploadedBy { get; set; } // e.g., "Dr. Sarah Chen"
        public DateTime Date { get; set; }
        public string DownloadUrl { get; set; }
    }
}