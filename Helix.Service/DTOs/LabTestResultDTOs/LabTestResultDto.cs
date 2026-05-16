using Helix.Data.Entities;
using System;

namespace Helix.Service.DTOs.LabTestResultDTOs
{
    public class LabTestResultDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string PatientName { get; set; }
        public string TerminologyName { get; set; }
        public DateTime CreateDate { get; set; }
        public string status { get; set; }
        public decimal value { get; set; }
        public string Unit { get; set; }
    }
}
