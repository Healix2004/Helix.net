using System.ComponentModel.DataAnnotations;

namespace Helix.Service.DTOs.ConsentDTOs
{
    public class ConsentRequestDto
    {
        // 1. WHAT are they sharing?
        // e.g., ["Radiology", "Labs", "Prescriptions", "FullRecord"]
        [Required(ErrorMessage = "You must specify at least one access scope.")]
        [MinLength(1, ErrorMessage = "Please select at least one record type to share.")]
        public List<string> Scopes { get; set; } = new List<string>();

        // 2. HOW LONG does the doctor have access?
        // Default is 120 minutes (2 hours), but the Angular app could let them choose.
        [Range(15, 1440, ErrorMessage = "Duration must be between 15 minutes and 24 hours.")]
        public int DurationInMinutes { get; set; } = 120;

        // 3. WHO is getting access? (Optional for QR Codes)
        // If the patient selects a doctor from a list, they send this ID.
        // If they are just holding up a QR code for ANY doctor in the room to scan, this stays null!
        public Guid? TargetDoctorId { get; set; }
    }
}
