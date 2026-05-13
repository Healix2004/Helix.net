using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Data.Entities
{
    public class LabImages :BaseEntity
    {
        public Guid LabTestResultId { get; set; } = Guid.Empty;
        public LabTestResult LabTestResult { get; set; } 
        public string ImageUrl { get; set; }
    }
}
