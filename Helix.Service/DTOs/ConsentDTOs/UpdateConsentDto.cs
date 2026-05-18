using System;

namespace Helix.Service.DTOs.ConsentDTOs
{
    public class UpdateConsentDto
    {
        public Guid Id { get; set; }
        public DateTime ConsentExpiresAt { get; set; }
        public bool IsEmergencyOverride { get; set; }
    }
}
