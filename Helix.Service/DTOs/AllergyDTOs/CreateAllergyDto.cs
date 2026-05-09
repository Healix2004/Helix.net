using System;

namespace Helix.Service.DTOs.AllergyDTOs
{
    public class CreateAllergyDto
    {
        public string name { get; set; }
        public string Category { get; set; }
        public DateTime RecordedDate { get; set; }
        public string criticality { get; set; }
        public Guid PatientId { get; set; }
    }
}
