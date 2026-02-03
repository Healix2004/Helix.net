using System.ComponentModel.DataAnnotations;

namespace Helix.Data.Entities
{
    public class MedicalConcept : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;
        [Required]
        [MaxLength(400)]
        public string Display { get; set; } = string.Empty;
        [Required]
        [MaxLength(200)]
        public string SystemUri { get; set; } = string.Empty;
        [MaxLength(50)]
        public string? Version { get; set; }
        public bool IsActive { get; set; } = true;
        public string? PropertiesJson { get; set; }
    }
}
