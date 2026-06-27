using Helix.Data.Enums;

namespace Helix.Service.DTOs.RadiologyOrderDto
{
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
        public EnRadiologyOrderStatus Status { get; set; } = EnRadiologyOrderStatus.Pending;
        public string StatusName => Status.ToString();
        public Guid? RadiologyResultId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
