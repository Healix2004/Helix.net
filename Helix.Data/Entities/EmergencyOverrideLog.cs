namespace Helix.Data.Entities
{
    public class EmergencyOverrideLog : BaseEntity
    {
        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }

        public string JustificationReason { get; set; } // e.g., "Patient unconscious, suspected cardiac arrest"
        public DateTime OverrideTimestamp { get; set; }

        // The override should expire automatically after a set time (e.g., 12 hours)
        public DateTime ExpirationTimestamp { get; set; }

        public bool IsReviewedByAdmin { get; set; } = false;
    }
}
