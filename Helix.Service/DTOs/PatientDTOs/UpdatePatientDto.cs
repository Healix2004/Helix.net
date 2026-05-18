using Helix.Data.Enums;

namespace Helix.Service.DTOs.PatientDTOs
{
    public class UpdatePatientDto
    {
        public Guid Id { get; set; }
        public EnPatientCategories PatientCategory { get; set; }
        public EnBloodTypes? BloodType { get; set; }
    }
}
