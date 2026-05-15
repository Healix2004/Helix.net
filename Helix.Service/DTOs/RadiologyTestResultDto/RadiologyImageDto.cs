namespace Helix.Service.DTOs.RadiologyTestResultDto
{
    // 3. The Read DTO for the Images (Helper for the master DTO below)
    public class RadiologyImageDto
    {
        public Guid Id { get; set; }
        public string FilePath { get; set; } = string.Empty; // e.g., "/uploads/radiology/scan1.jpg"
        public string FileName { get; set; } = string.Empty; // e.g., "scan1.jpg"
        public string? Label { get; set; }
    }
}
