namespace Helix.Service.DTOs.AppointmentDtos
{
    public class AvailableDaysDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        // Returns a list of day numbers (1-31) that have AT LEAST ONE available slot
        public List<int> AvailableDays { get; set; } = new();
    }
}
