using Helix.Service.DTOs.ChatDTOs;
using Helix.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Helix.Service.Services.ChatbotService
{
    public class ChatbotService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : IChatbotService
    {
        public async Task<ChatResponseDto> SendMessageAsync(ChatRequestDto request)
        {
            var client = httpClientFactory.CreateClient();

            // This will read the environment variable first. 
            // If it's missing (like when running locally outside Docker), it falls back to localhost.
            var chatbotApiUrl = configuration["ChatbotApiUrl"] ?? "http://helix.ai.chatbot:5432/api/chat";

            try
            {
                var response = await client.PostAsJsonAsync(chatbotApiUrl, request);
                response.EnsureSuccessStatusCode();

                // Assuming your bot returns { "reply": "Hello, Doctor!" }
                var botReply = await response.Content.ReadFromJsonAsync<ChatResponseDto>();

                return botReply ?? new ChatResponseDto { IsSuccess = false, Reply = "Empty response." };
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ChatResponseDto
                {
                    IsSuccess = false,
                    Reply = "I am currently offline or experiencing network issues."
                };
            }
        }
    }
}
