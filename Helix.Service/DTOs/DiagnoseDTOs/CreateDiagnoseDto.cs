using Helix.Data.Entities;
using System;

namespace Helix.Service.DTOs.DiagnoseDTOs
{
    public class CreateDiagnoseDto
    {
        public Guid TerminologyCodeId { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public DateOnly dateOnly { get; set; }
        public string Notes { get; set; }
        public EnStatus status { get; set; }
    }
}
