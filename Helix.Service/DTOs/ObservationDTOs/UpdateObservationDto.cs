using System;

namespace Helix.Service.DTOs.ObservationDTOs
{
    public class UpdateObservationDto
    {
        public Guid Id { get; set; }
        public string name { get; set; }
        public decimal? value { get; set; }
        public string unit { get; set; }
    }
}
