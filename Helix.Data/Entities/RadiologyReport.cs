using System;
using System.Collections.Generic;

namespace Helix.Data.Entities
{
    public class RadiologyReport : BaseEntity
    {
        public RadiologyReport()
        {
            ImageUrls = new List<string>();
        }

        // --- Foreign Keys ---
        public Guid RadiologyOrderId { get; set; }

        // We now just store the name of the external doctor as plain text.
        public string? ExternalRadiologistName { get; set; }

        // --- Clinical Narrative ---
        public string Findings { get; set; } = default!;
        public string Conclusion { get; set; } = default!;

        // --- Imaging References ---
        public List<string> ImageUrls { get; set; }
        public string? DicomStudyInstanceUid { get; set; }

        public DateTime ReportDate { get; set; } = DateTime.UtcNow;

        // --- Navigation Properties ---
        public virtual RadiologyOrder RadiologyOrder { get; set; } = null!;
    }
}