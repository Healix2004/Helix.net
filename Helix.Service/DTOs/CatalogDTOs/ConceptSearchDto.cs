using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Service.DTOs.CatalogDTOs
{
    public class ConceptSearchDto
    {
        public Guid ConceptId { get; set; }
        public string Code { get; set; }
        public string DisplayName { get; set; }
        public bool IsPanel { get; set; }
        public bool IsRadiology { get; set; }
    }
}
