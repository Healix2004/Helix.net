using Azure.Core;
using Helix.Api.Base;
using Helix.Core.Bases;
using Helix.Core.Features.Auth.Commands.Models;
using Helix.Core.Features.Auth.Quieres.Models;
using Helix.Service.DTOs.AuthDTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Helix.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : AppControllerBase
    {
        [HttpPost("login")]
        [ProducesResponseType(typeof(Response<AuthDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<AuthDto>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Response<AuthDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var command = new LoginCommand(dto);
            var result = await mediator.Send(command);

            return StatusCode((int)(result.StatusCode), result);
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(Response<AuthDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Response<AuthDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromForm] RegisterDto dto)
        {
            var command = new RegisterCommand(dto);
            var result = await mediator.Send(command);

            if (!result.Succeeded)
            {
                return StatusCode((int)(result.StatusCode), result);
            }

            return StatusCode((int)(result.StatusCode), result);
        }
        [HttpPost("register_step1")]
        [ProducesResponseType(typeof(Response<AuthDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Response<AuthDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterStep1([FromForm] RegisterStep1Dto dto)
        {
            var command = new RegisterStep1Command(dto);
            var result = await mediator.Send(command);
            if(!result.Succeeded)
            {
                return StatusCode((int)(result.StatusCode), result);
            }
            return StatusCode((int)(result.StatusCode), result);
        }
        [HttpPost("registerDoctor")]
        [ProducesResponseType(typeof(Response<AuthDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Response<AuthDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterDoctor([FromForm] RegisterDoctorDto dto)
        {
            var command = new RegisterDoctorCommand(dto);
            var result = await mediator.Send(command);
            if(!result.Succeeded)
            {
                return StatusCode((int)(result.StatusCode), result);
            }
            return StatusCode((int)(result.StatusCode), result);
        }

        [HttpGet("profile/{userId}")]
        [Authorize]
        [ProducesResponseType(typeof(Response<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<UserDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserProfile(string userId)
        {
            var query = new GetUserProfileQuery(userId);
            var result = await mediator.Send(query);

            return StatusCode((int)(result.StatusCode), result);
        }

        [HttpGet("user/{email}")]
        [Authorize]
        [ProducesResponseType(typeof(Response<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<UserDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var query = new GetUserByEmailQuery(email);
            var result = await mediator.Send(query);

            return StatusCode((int)(result.StatusCode), result);
        }

        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgetPasswordDto dto)
        {
            var command = new ForgetPasswordCommand(dto);
            var result = await mediator.Send(command);

            return StatusCode((int)(result.StatusCode), result);
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var command = new ResetPasswordCommand(dto);
            var result = await mediator.Send(command);

            return StatusCode((int)(result.StatusCode), result);
        }

        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            // Get user ID from claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Response<string>
                {
                    StatusCode = System.Net.HttpStatusCode.Unauthorized,
                    Succeeded = false,
                    Message = "User ID not found in token"
                });
            }

            var command = new ChangePasswordCommand(dto, userId);
            var result = await mediator.Send(command);

            return StatusCode((int)(result.StatusCode), result);
        }

        [HttpPost("confirm-email")]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto dto)
        {
            var command = new ConfirmEmailCommand(dto);
            var result = await mediator.Send(command);

            return StatusCode((int)(result.StatusCode), result);
        }
    }
}
