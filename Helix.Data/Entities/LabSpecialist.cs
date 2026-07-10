namespace Helix.Data.Entities
{
    public class LabSpecialist : BaseEntity
    {
        // --- Relational Properties ---
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; } 

        // --- Core Information ---
        public string FullName { get; set; }
        public string NationalId { get; set; }
        public string LabName { get; set; }
        public string Address { get; set; }
        public string LicenseNumber { get; set; }

        public bool IsVerified { get; set; } = false;

        // --- Document URLs ---
        public string? ProfileImageUrl { get; set; }
        public string LabSpecialistLicenseUrl { get; set; }
        public string NationalIdUrl { get; set; }
    }
}
