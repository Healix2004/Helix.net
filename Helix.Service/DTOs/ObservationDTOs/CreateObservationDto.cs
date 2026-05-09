using System;

namespace Helix.Service.DTOs.ObservationDTOs
{
    public class CreateObservationDto
    {
        public string name { get; set; }
        public decimal? value { get; set; }
        public string unit { get; set; }
        public Guid EncounterId { get; set; }
    }
}
