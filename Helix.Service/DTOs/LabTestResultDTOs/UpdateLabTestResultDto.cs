using Helix.Data.Entities;

namespace Helix.Service.DTOs.LabTestResultDTOs
{
    public class UpdateLabTestResultDto
    {
        public EnStatus status { get; set; }
        public decimal value { get; set; }
        public string Unit { get; set; }
    }
}
