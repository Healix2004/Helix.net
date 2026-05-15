namespace Helix.Data.Entities
{
    public class RadiologyImage : BaseEntity
    {
        public Guid RadiologyResultId { get; set; }
        public string FilePath { get; set; } = string.Empty;  // Relative path on server
        public string FileName { get; set; } = string.Empty;  // Original name
        public string? Label { get; set; }                    // e.g., "Left Side View"
        public long FileSizeInKB { get; set; }
    }
}
