using Helix.Service.DTOs.AuthDTOs;

namespace Helix.Service.DTOs.DoctorDTOs
{
    public class DoctorRegistrationPayloadDto
    {
        // 1. The standard account data (Username, Password, Email, Profile Image)
        public RegisterDto AccountDetails { get; set; }

        // 2. The professional data (Specialty, Availability, Uploaded Documents)
        public CreateDoctorDto ProfessionalDetails { get; set; }
    }
}