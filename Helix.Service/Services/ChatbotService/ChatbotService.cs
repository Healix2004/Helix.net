using Helix.Service.DTOs.ChatDTOs;
using Helix.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Helix.Service.Services.ChatbotService
{
    public class ChatbotService(IHttpClientFactory httpClientFactory) : IChatbotService
    {
        public async Task<ChatResponseDto> SendMessageAsync(ChatRequestDto request)
        {
            var client = httpClientFactory.CreateClient();

            // NOTE: Replace this URL with your actual chatbot's API endpoint
            var chatbotApiUrl = "http://helix.ai.chatbot:8000/api/chat";

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
