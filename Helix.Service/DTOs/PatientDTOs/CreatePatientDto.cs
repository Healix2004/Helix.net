using Helix.Data.Enums;

namespace Helix.Service.DTOs.PatientDTOs
{
    public class CreatePatientDto
    {
        public string AppUserId { get; set; }
        public EnPatientCategories PatientCategory { get; set; }
        public EnBloodTypes? BloodType { get; set; }
    }
}
