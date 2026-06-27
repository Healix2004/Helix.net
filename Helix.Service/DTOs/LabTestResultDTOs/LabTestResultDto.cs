using Helix.Data.Entities;
using Helix.Data.Enums;
using System;

namespace Helix.Service.DTOs.LabTestResultDTOs
{
    public class LabTestResultDto
    {
        public Guid ResultId { get; set; }

        // Flattened Medical Concept Data for easy UI rendering
        public Guid MedicalConceptId { get; set; }
        public string TestCode { get; set; } // e.g., "718-7"
        public string TestName { get; set; } // e.g., "Hemoglobin"

        // Core Result Data
        public EnLabOrderStatus Status { get; set; }
        public decimal? NumericValue { get; set; }
        public string? StringValue { get; set; }
        public string? Unit { get; set; }
        public string? ReferenceRange { get; set; }
        public string? InterpretationFlag { get; set; }
        public DateTime? ResultDate { get; set; }
    }
}
