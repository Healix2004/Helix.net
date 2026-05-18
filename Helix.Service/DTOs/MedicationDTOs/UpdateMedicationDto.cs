using System;

namespace Helix.Service.DTOs.MedicationDTOs
{
    public class UpdateMedicationDto
    {
        public Guid Id { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public DateTime EndDate { get; set; }
    }
}
