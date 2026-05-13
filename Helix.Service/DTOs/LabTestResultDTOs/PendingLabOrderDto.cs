namespace Helix.Service.DTOs.LabTestResultDTOs
{
    public class PendingLabOrderDto
    {
        public Guid OrderId { get; set; }
        public string TestName { get; set; }
        public string QrToken { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
