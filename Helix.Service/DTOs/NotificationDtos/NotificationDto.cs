using Helix.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Service.DTOs.NotificationDtos
{
    public class NotificationDto
    {
        public string AppUserId { get; set; } // The user receiving the alert
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
