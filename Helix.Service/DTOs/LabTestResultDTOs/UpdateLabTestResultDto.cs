using Helix.Data.Entities;

namespace Helix.Service.DTOs.LabTestResultDTOs
{
    public class UpdateLabTestResultDto
    {
        public Guid Id { get; set; }
        public EnStatus status { get; set; }
        public decimal value { get; set; }
        public string Unit { get; set; }
    }
}
