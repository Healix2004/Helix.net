using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Data.Entities
{
    public class LabImages
    {
        public Guid LabTestResultId { get; set; }
        public LabTestResult LabTestResult { get; set; } 
        
        public string ImageUrl { get; set; }
    }
}
