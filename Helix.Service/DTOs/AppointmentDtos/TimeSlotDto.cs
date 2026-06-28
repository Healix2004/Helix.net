using Helix.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Service.DTOs.AppointmentDtos
{
    // Used for the specific time buttons
    public class TimeSlotDto
    {
        // For the UI display (e.g., "08:30 AM")
        public string DisplayTime { get; set; } = string.Empty;

        // The actual DateTimes the frontend will send back in the POST payload when booking
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // Tells the frontend whether to leave the button clickable or "cross it out"
        public bool IsAvailable { get; set; }
    }
}
