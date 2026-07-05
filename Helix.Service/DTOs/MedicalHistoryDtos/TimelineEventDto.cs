using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Service.DTOs.MedicalHistoryDtos
{
    public class TimelineEventDto
    {
        public Guid Id { get; set; }

        // Tells Angular what type of event this is (e.g., "Appointment", "Lab", "Radiology")
        public string EventType { get; set; }

        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Provider { get; set; }
        public List<TimelineDetailDto> Details { get; set; } = new();
    }

    public class TimelineDetailDto
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }
}
