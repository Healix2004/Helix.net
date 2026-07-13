using Helix.Api.Base;
using Helix.Core.Bases; // ADDED: For manual Response<T> wrappers
using Helix.Data.Enums;
using Helix.Service.DTOs.ChatDTOs;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    [Route("api/chatbot")]
    [ApiController]
    [Authorize(Roles = $"{nameof(EnRoles.Doctor)},{nameof(EnRoles.Patient)}")]
    public class ChatbotController(IChatbotService chatbotService) : AppControllerBase
    {
        [HttpPost("ask")]
        public async Task<IActionResult> AskBot([FromBody] ChatRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
            {
                return BadRequest("Prompt cannot be empty.");
            }

            var response = await chatbotService.SendMessageAsync(request);

            return NewResult(new Response<ChatResponseDto>(response)
            {
                Succeeded = response.IsSuccess,
                StatusCode = response.IsSuccess ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ServiceUnavailable,
                Message = response.IsSuccess ? "Success" : "Bot failed to reply."
            });
        }
    }
}