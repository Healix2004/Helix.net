using Helix.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Helix.Service.DTOs.LabOrderDTOs
{
    public class UpdateLabOrderDto
    {
        [Required(ErrorMessage = "Lab Order ID is required.")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "You must select a specific lab test (Terminology Code).")]
        public TerminologyCodeLookup TerminologyCodeId { get; set; }
    }
}
