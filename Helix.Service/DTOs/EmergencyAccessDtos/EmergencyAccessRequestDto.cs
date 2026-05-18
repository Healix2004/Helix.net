using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Service.DTOs.EmergencyAccessDtos
{
    public class EmergencyAccessRequestDto
    {
        public Guid PatientId { set; get; }
        public string Reason { set; get; }
    }
}
