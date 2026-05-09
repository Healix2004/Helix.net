using Helix.Data.Enums;

namespace Helix.Service.DTOs.TerminologyCodeLookupDTOs
{
    public class CreateTerminologyCodeLookupDto
    {
        public string Code { get; set; }
        public string Display { get; set; }
        public string SystemUrl { get; set; }
        public EnTerminologyType TerminologyType { get; set; }
        public long UsageCount { get; set; }
    }
}
