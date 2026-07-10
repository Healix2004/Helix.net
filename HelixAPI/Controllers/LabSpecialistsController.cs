using Helix.Api.Base;
using Helix.Core.Bases;
using Helix.Service.DTOs.LabSpecialistDto;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Helix.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LabSpecialistsController : AppControllerBase
    {
        private readonly ILabSpecialistService _labSpecialistService;

        public LabSpecialistsController(ILabSpecialistService labSpecialistService)
        {
            _labSpecialistService = labSpecialistService;
        }
        [HttpPost("register")]
        [Authorize]
        public async Task<IActionResult> Register([FromForm] RegisterLabSpecialistDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid appUserId))
                {
                    return NewResult(new Response<string>("User profile could not be verified.")
                    {
                        Succeeded = false,
                        StatusCode = System.Net.HttpStatusCode.Unauthorized
                    });
                }

                // 2. Call the service to process the registration and save to the database
                // Note: Make sure RegisterLabSpecialistAsync now accepts a Guid for appUserId. 
                // If it still expects a string, pass appUserId.ToString() instead.
                var labSpecialist = await _labSpecialistService.RegisterLabSpecialistAsync(dto, appUserId.ToString());

                // 3. Return a successful standardized response
                return NewResult(new Response<string>("Lab Specialist profile registered successfully and is pending admin verification.")
                {
                    Succeeded = true,
                    StatusCode = System.Net.HttpStatusCode.Created
                });
            }
            catch (InvalidOperationException ex)
            {
                // Catches the specific error if the user already has a profile registered
                return NewResult(new Response<string>(ex.Message)
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }
            catch (Exception ex)
            {
                // Fallback for unexpected errors (e.g., database or file system issues)
                return NewResult(new Response<string>("An error occurred while processing your registration.")
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Errors = new System.Collections.Generic.List<string> { ex.Message }
                });
            }
        }
    }
}