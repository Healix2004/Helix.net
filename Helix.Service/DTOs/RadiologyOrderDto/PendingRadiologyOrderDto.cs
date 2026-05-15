using Helix.Data.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Helix.Service.DTOs.RadiologyOrderDto
{
    // 1. Used when a Doctor creates a brand new order
    public class CreateRadiologyOrderDto
    {
        [Required(ErrorMessage = "Patient is required.")]
        public Guid PatientId { get; set; }

        [Required(ErrorMessage = "Doctor is required.")]
        public Guid DoctorId { get; set; }

        [Required(ErrorMessage = "You must select a specific scan (e.g., MRI Brain).")]
        public Guid TerminologyCodeId { get; set; }
    }

    // 2. Used if a Doctor needs to edit an order before it is completed
    public class UpdateRadiologyOrderDto
    {
        [Required]
        public Guid Id { get; set; }
        public Guid? TerminologyCodeId { get; set; }
        public EnLabOrderStatus? Status { get; set; }
    }

    // 3. Used for the Technician/Radiologist dashboard when scanning the QR code
    // This is lightweight and only contains what the scan center needs to see
    public class PendingRadiologyOrderDto
    {
        public Guid Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string RequestingDoctorName { get; set; } = string.Empty;
        public string TerminologyDisplay { get; set; } = string.Empty;
        public string QrToken { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    // 4. The Master DTO used when retrieving full details of an order
    public class RadiologyOrderDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public Guid TerminologyCodeId { get; set; }
        public string TerminologyDisplay { get; set; } = string.Empty;
        public string TerminologyCode { get; set; } = string.Empty; // e.g., "70551"
        public string QrToken { get; set; } = string.Empty;
        public EnLabOrderStatus Status { get; set; } = EnLabOrderStatus.Pending;
        public string StatusName => Status.ToString();
        public Guid? RadiologyResultId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
