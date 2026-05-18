using Helix.Data.Entities;
using System;

namespace Helix.Service.DTOs.DiagnoseDTOs
{
    public class UpdateDiagnoseDto
    {
        public Guid Id { get; set; }
        public string Notes { get; set; }
        public EnStatus status { get; set; }
    }
}
