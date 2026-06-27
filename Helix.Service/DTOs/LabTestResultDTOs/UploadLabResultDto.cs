namespace Helix.Service.DTOs.LabTestResultDTOs
{
    public class UploadLabResultDto
    {
        public Guid OrderId { get; set; }
        // Send a list of results so panels can be updated in one request
        public List<LabTestResultItemDto> Results { get; set; } = new List<LabTestResultItemDto>();
    }
    public class LabTestResultItemDto
    {
        public Guid MedicalConceptId { get; set; } // Identifies which empty slot to fill

        public decimal? NumericValue { get; set; }
        public string? StringValue { get; set; } // For results like "Positive" or "Negative"
        public string? Unit { get; set; }
        public string? ReferenceRange { get; set; } // e.g., "4.5 - 11.0"
        public string? InterpretationFlag { get; set; } // e.g., "High", "Low", "Abnormal"
    }
}
