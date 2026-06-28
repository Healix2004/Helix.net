using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Helix.Service.DTOs.PrescriptionDtos
{
    // The main payload sent when the doctor clicks "Save Prescription"
    public class CreatePrescriptionDto
    {
        [Required]
        public Guid PatientId { get; set; }

        // The frontend passes the Appointment ID this was prescribed during
        public Guid? AppointmentId { get; set; }

        public string? DoctorNotes { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "You must prescribe at least one medication.")]
        public List<CreatePrescriptionItemDto> Medications { get; set; } = new List<CreatePrescriptionItemDto>();
    }
}
