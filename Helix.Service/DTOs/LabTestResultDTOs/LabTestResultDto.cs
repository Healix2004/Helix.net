using Helix.Data.Entities;
using System;

namespace Helix.Service.DTOs.LabTestResultDTOs
{
    public class LabTestResultDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid TerminologyCodeId { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid EncounterId { get; set; }
        public EnStatus status { get; set; }
        public decimal value { get; set; }
        public string Unit { get; set; }
    }
}
