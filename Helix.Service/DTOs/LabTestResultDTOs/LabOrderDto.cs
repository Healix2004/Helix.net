namespace Helix.Service.DTOs.LabTestResultDTOs
{
    public class LabOrderDto
    {
        public Guid OrderId { get; set; }
        public string PatientName { get; set; }
        public string TestCode { get; set; }
        public string TestName { get; set; }
    }
}
