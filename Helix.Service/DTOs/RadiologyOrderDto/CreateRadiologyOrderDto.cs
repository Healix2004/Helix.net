using System.ComponentModel.DataAnnotations;

namespace Helix.Service.DTOs.RadiologyOrderDto
{
    // 1. Used when a Doctor creates a brand new order
    public class CreateRadiologyOrderDto
    {
        [Required(ErrorMessage = "Patient is required.")]
        public Guid PatientId { get; set; }

        [Required(ErrorMessage = "Doctor is required.")]
        public Guid DoctorId { get; set; }

        [Required(ErrorMessage = "Prescription ID is required.")]
        public Guid PrescriptionId { get; set; }

        [Required(ErrorMessage = "You must select a specific scan (e.g., MRI Brain).")]
        public string TerminologyCode { get; set; } = string.Empty;
    }
}
