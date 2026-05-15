namespace Helix.Service.DTOs.RadiologyTestResultDto
{
    // 4. The Master Read DTO used to display the final report to the Patient or Doctor
    public class RadiologyTestResultDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public Guid RadiologistId { get; set; }
        public string RadiologistName { get; set; } = string.Empty;
        public Guid TerminologyCodeId { get; set; }
        public string TerminologyDisplay { get; set; } = string.Empty; // e.g., "MRI Brain without contrast"
        public string Findings { get; set; } = string.Empty;
        public string Impression { get; set; } = string.Empty;
        public DateTime PerformedDate { get; set; }
        public List<RadiologyImageDto> Images { get; set; } = new List<RadiologyImageDto>();
    }
}
