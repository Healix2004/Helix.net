using System.ComponentModel.DataAnnotations;

namespace Helix.Service.DTOs.RadiologyTestResultDto
{
    // 2. Used if the Radiologist makes a typo and needs to edit their report
    public class UpdateRadiologyTestResultDto
    {
        [Required]
        public Guid Id { get; set; }
        public string? Findings { get; set; }
        public string? Impression { get; set; }
    }
}
