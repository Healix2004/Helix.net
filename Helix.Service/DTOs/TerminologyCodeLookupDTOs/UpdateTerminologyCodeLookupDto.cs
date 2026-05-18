using Helix.Data.Enums;

namespace Helix.Service.DTOs.TerminologyCodeLookupDTOs
{
    public class UpdateTerminologyCodeLookupDto
    {
        public Guid Id { get; set; }
        public string Display { get; set; }
        public EnTerminologyType TerminologyType { get; set; }
        public long UsageCount { get; set; }
    }
}
