using System;

namespace Helix.Service.DTOs.AllergyDTOs
{
    public class UpdateAllergyDto
    {
        public string name { get; set; }
        public string Category { get; set; }
        public DateTime RecordedDate { get; set; }
        public string criticality { get; set; }
    }
}
