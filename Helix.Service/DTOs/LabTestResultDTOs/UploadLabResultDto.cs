using Helix.Data.Entities;

namespace Helix.Service.DTOs.LabTestResultDTOs
{
    // Command request body DTO for the API Controller
    public class UploadLabResultDto
    {
        public Guid OrderId { get; set; }
        public decimal Value { get; set; }
        public string Unit { get; set; }

    }
}
