using Helix.Data.Enums;
using System;

namespace Helix.Service.DTOs.TerminologyCodeLookupDTOs
{
    public class TerminologyCodeLookupDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Display { get; set; }
        public string SystemUrl { get; set; }
        public EnTerminologyType TerminologyType { get; set; }
        public long UsageCount { get; set; }
    }
}
