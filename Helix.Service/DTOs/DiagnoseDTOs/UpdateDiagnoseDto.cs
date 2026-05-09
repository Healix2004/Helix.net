using Helix.Data.Entities;
using System;

namespace Helix.Service.DTOs.DiagnoseDTOs
{
    public class UpdateDiagnoseDto
    {
        public string Notes { get; set; }
        public EnStatus status { get; set; }
    }
}
