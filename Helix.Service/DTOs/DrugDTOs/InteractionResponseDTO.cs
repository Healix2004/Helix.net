using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Helix.Service.DTOs.DrugDTOs
{
    public class InteractionResponseDTO
    {
        public string DrugA { get; set; } = string.Empty;
        public string DrugB { get; set; } = string.Empty;
        public bool IsInteraction { get; set; }
        public double Confidence { get; set; }

        // The clean message to display to the doctor
        public string Message { get; set; } = string.Empty;
    }
    internal class RawAiResponseDTO
    {
        [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
        [JsonPropertyName("drug_a")] public string DrugA { get; set; } = string.Empty;
        [JsonPropertyName("drug_b")] public string DrugB { get; set; } = string.Empty;
        [JsonPropertyName("result")] public string Result { get; set; } = string.Empty;
    }
}
