using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Helix.Service.DTOs.ChatDTOs
{
    public class ChatResponseDto
    {
        [JsonPropertyName("reply")]
        public string Reply { get; set; } = string.Empty;

        [JsonIgnore]
        public bool IsSuccess { get; set; } = true;
    }
    public class ChatRequestDto
    {
        // Example: If Python expects "message" instead of "prompt"
        [JsonPropertyName("message")]
        public string Prompt { get; set; } = string.Empty;

        // Example: If Python expects "chat_history" instead of "chatHistory"
        [JsonPropertyName("chat_history")]
        public List<ChatMessageDto> ChatHistory { get; set; } = new List<ChatMessageDto>();
    }

    public class ChatMessageDto
    {
        // Python often expects lowercase "role" and "content"
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }
}
