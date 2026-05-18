using Helix.Api.Base;
using Helix.Core.Bases;
using Helix.Data.Enums;
using Helix.Service.DTOs.EmergencyAccessDtos;
using Helix.Service.Helper;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    /// <summary>
    /// Manages "Break the Glass" emergency access protocols for the HELIX system.
    /// </summary>
    [Route("api/emergency-access")]
    [ApiController]
    public class EmergencyAccessController(IEmergencyAccessService emergencyAccessService,IDoctorService doctorService) : AppControllerBase
    {
        [HttpPost("break-the-glass")]
        [Authorize(Roles = nameof(EnRoles.Doctor))]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BreakTheGlass([FromBody] EmergencyAccessRequestDto request)
        {
            // 1. Safely extract the Doctor's identity from their JWT
            Guid doctorId = await User.GetDoctorIdAsync(doctorService);

            // 2. Trigger the emergency override and audit logging
            await emergencyAccessService.ActivateEmergencyOverrideAsync(doctorId, request.PatientId, request.Reason);

            // 3. Return the standard HELIX Response<T> wrapper
            var successResponse = new Response<string>("Emergency access activated.")
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "Emergency access granted for 12 hours. This action has been audited and administrators have been notified."
            };

            return NewResult(successResponse);
        }
    }
}