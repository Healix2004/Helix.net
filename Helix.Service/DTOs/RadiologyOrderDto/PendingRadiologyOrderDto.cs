using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Service.DTOs.RadiologyOrderDto
{

    // This is lightweight and only contains what the scan center needs to see
    public class PendingRadiologyOrderDto
    {
        public Guid Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string RequestingDoctorName { get; set; } = string.Empty;
        public string TerminologyDisplay { get; set; } = string.Empty;
        public string QrToken { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
