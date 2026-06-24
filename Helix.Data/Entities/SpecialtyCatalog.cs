using System.ComponentModel.DataAnnotations;

namespace Helix.Data.Entities
{
    public class SpecialtyCatalog
    {
        public string Code { get; set; } // The SNOMED Code (e.g., 394579002)
        public string DisplayName { get; set; } // e.g., "Cardiology"
        public string CodeSystem { get; set; } = "SNOMED";
    }
}
