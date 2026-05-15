namespace Helix.Data.Entities
{
    public class RadiologyResult : BaseEntity
    {
        public Guid PatientId { get; set; }
        public Guid OrderId { get; set; }
        public string StudyType { get; set; } = string.Empty; // e.g., "Chest X-Ray"
        public string Findings { get; set; } = string.Empty;  // Detailed observations
        public string Impression { get; set; } = string.Empty; // Summary conclusion
        public DateTime PerformedDate { get; set; } = DateTime.Now;
        public RadiologyOrder? Order { get; set; }
        // Navigation for images
        public virtual ICollection<RadiologyImage> Images { get; set; } = new List<RadiologyImage>();
    }
}
