using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Service.DTOs.DrugDTOs
{
    public class InteractionResponseDTO
    {
       public string status { get; set; }
       public string drug_a { get; set; }
       public string drug_b { get; set; }
       public string result { get; set; }
    }
}
