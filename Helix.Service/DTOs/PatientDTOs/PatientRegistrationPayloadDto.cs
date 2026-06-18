using Helix.Service.DTOs.AuthDTOs;

namespace Helix.Service.DTOs.PatientDTOs
{
    public class PatientRegistrationPayloadDto
    {
        // 1. The Account Data (from the Canvas document)
        public RegisterDto AccountDetails { get; set; }

        // 2. The Medical Data (the CreatePatientDto we made earlier)
        // Note: You can remove the 'AppUserId' property from CreatePatientDto 
        // because we won't know it until AFTER the account is created!
        public CreatePatientDto MedicalDetails { get; set; }
    }
}