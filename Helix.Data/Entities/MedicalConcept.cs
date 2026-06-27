using System.ComponentModel.DataAnnotations;

namespace Helix.Data.Entities
{
    public class MedicalConcept :BaseEntity
    {
        [Required]
        [MaxLength(20)]
        public string Code { get; set; } // Corresponds to LOINC_NUM

        [Required]
        [MaxLength(255)]
        public string SystemUri { get; set; } = "http://loinc.org"; // Standard LOINC system URI

        [MaxLength(255)]
        public string Component { get; set; } = string.Empty;
        [MaxLength(255)]
        public string Property { get; set; } = string.Empty;
        [MaxLength(255)]
        public string TimeAspect { get; set; } = string.Empty;
        [MaxLength(255)]
        public string System { get; set; } = string.Empty;
        [MaxLength(255)]
        public string ScaleType { get; set; } = string.Empty;
        [MaxLength(255)]
        public string MethodType { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Class { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Display { get; set; } // Corresponds to LONG_COMMON_NAME

        public bool IsActive { get; set; } = true;
        public bool IsRadiology { get; set; } = false;

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        // Navigation properties for panel relationships
        public ICollection<LoincPanelComponent> ParentPanels { get; set; } // Panels where this concept is a child
        public ICollection<LoincPanelComponent> ChildComponents { get; set; } // Components of this panel
    }
}
