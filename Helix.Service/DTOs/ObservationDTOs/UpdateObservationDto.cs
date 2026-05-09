using System;

namespace Helix.Service.DTOs.ObservationDTOs
{
    public class UpdateObservationDto
    {
        public string name { get; set; }
        public decimal? value { get; set; }
        public string unit { get; set; }
    }
}
