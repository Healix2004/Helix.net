using System;

namespace Helix.Service.DTOs.MedicationDTOs
{
    public class CreateMedicationDto
    {
        public Guid TerminologyCodeId { get; set; }
        public Guid PatientId { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public DateOnly StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
