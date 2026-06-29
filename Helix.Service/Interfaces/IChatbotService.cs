using Helix.Service.DTOs.ChatDTOs;

namespace Helix.Service.Interfaces
{
    public interface IChatbotService
    {
        Task<ChatResponseDto> SendMessageAsync(ChatRequestDto request);
    }
}
