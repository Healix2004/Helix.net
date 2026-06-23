using System.ComponentModel.DataAnnotations;

namespace Helix.Service.DTOs.PatientDTOs
{
    public class CreateEmergencyContactDto
    {
        [Required(ErrorMessage = "Emergency contact name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Emergency contact phone number is required")]
        public string PhoneNumber { get; set; }
    }
}