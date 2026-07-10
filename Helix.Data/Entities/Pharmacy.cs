namespace Helix.Data.Entities
{
    public class Pharmacy: BaseEntity
    {
        public string AppUserId { get; set; }
        public virtual AppUser AppUser { get; set; }

        // --- Shared Fields (Required by both) ---
        public string FullName { get; set; } = string.Empty;
        public string NationalId { get; set; } = default!;
        public string PharmacyName { get; set; }
        public string Address { get; set; }
        public string LicenseNumber { get; set; }
        public bool IsVerified { get; set; } = false;

        // --- Combined Document Logic ---
        // Instead of locking into specific ID types, use generic document slots
        public string? ProfileImageUrl { get; set; }
        public string PrimaryLicenseUrl { get; set; } // Web: Pharmacy License | Mobile: License Card
        public string NationalIdUrl { get; set; }   // Web: Gov ID | Mobile: National ID
    }
}
