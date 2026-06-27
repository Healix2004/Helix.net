using Helix.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Helix.Data.Entities
{
    public class TerminologyCodeLookup :BaseEntity
    {
        public string Code { get; set; }
        public string Display {  get; set; }
        public string SystemUrl { get; set; }
        public EnTerminologyType TerminologyType { get; set; }
        public long UsageCount { get; set; } = 0;
    }
}
