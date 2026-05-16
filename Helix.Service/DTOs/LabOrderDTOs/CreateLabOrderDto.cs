using Helix.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Helix.Service.DTOs.LabOrderDTOs
{
    public class CreateLabOrderDto
    {
        [Required(ErrorMessage = "Patient ID is required.")]
        public Guid PatientId { get; set; }

        [Required(ErrorMessage = "Doctor ID is required.")]
        public Guid DoctorId { get; set; }

        [Required(ErrorMessage = "You must select a specific lab test (Terminology Code).")]
        public string TerminologyCode { get; set; }
    }
}
