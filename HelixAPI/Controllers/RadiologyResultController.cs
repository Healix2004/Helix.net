using Helix.Api.Base;
using Helix.Core.Bases;
using Helix.Data.Enums;
using Helix.Service.DTOs.RadiologyTestResultDto;
using Helix.Service.Helper;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    [Route("api/radiology-results")]
    [ApiController]
    public class RadiologyResultController(IRadiologyResultService radiologyResultService, IPatientService patientService) : AppControllerBase
    {
        // ==========================================
        // PATIENT & DOCTOR WORKFLOW (Retrieving)
        // ==========================================

        [HttpGet("order/{orderId}")]
        [Authorize] // CRITICAL FIX: Never leave medical records unauthenticated!
        public async Task<IActionResult> GetResultByOrderId(Guid orderId)
        {
            var result = await radiologyResultService.GetRadiologyResultByOrderIdAsync(orderId);

            var response = new Response<RadiologyTestResultDto>(result)
            {
                Succeeded = result != null,
                StatusCode = result != null ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.NotFound,
                Message = result != null ? "Radiology result retrieved successfully." : "No result found for this order."
            };
            return NewResult(response);
        }

        // ==========================================================
        // 1. FOR THE PATIENT (No Consent Needed)
        // ==========================================================
        [HttpGet("my-radiology")]
        [Authorize(Roles = nameof(EnRoles.Patient))]
        public async Task<IActionResult> GetMyRadiologyRecords()
        {
            // UPGRADE: Using your new Extension Method to prevent deadlocks and controller bloat
            Guid patientId = await User.GetPatientIdAsync(patientService);

            var results = await radiologyResultService.GetRadiologyResultsByPatientIdAsync(patientId);

            var response = new Response<List<RadiologyTestResultDto>>(results)
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "Your radiology records retrieved successfully."
            };
            return NewResult(response);
        }

        // ==========================================================
        // 2. FOR THE DOCTOR (Requires the 2-Hour Consent Token)
        // ==========================================================
        [HttpGet("patient/{patientId}/radiology")]
        [Authorize(Roles = nameof(EnRoles.Doctor))]
        public async Task<IActionResult> GetPatientRadiologyRecords(Guid patientId)
        {
            if (!User.HasValidConsent(patientId, "Radiology"))
            {
                return Forbid("You do not have active consent to view these records.");
            }

            var results = await radiologyResultService.GetRadiologyResultsByPatientIdAsync(patientId);

            var response = new Response<List<RadiologyTestResultDto>>(results)
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "Patient radiology results retrieved successfully."
            };
            return NewResult(response);
        }

        // ==========================================
        // ADMIN & RADIOLOGIST WORKFLOW (Management)
        // ==========================================

        [HttpGet("all")]
        [Authorize(Roles = nameof(EnRoles.Admin))]
        public async Task<IActionResult> GetAllResults()
        {
            var results = await radiologyResultService.GetAllRadiologyResultsAsync();
            var response = new Response<List<RadiologyTestResultDto>>(results)
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "All radiology results retrieved successfully."
            };
            return NewResult(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{nameof(EnRoles.Doctor)},{nameof(EnRoles.Admin)}")]
        public async Task<IActionResult> UpdateRadiologyResult(Guid id, [FromBody] UpdateRadiologyTestResultDto dto)
        {
            if (id != dto.Id)
            {
                var badResponse = new Response<bool>(false)
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = "The ID in the URL does not match the ID in the body."
                };
                return NewResult(badResponse);
            }

            await radiologyResultService.UpdateRadiologyResultAsync(dto);

            var response = new Response<bool>(true)
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "Radiology result updated successfully."
            };
            return NewResult(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(EnRoles.Admin))]
        public async Task<IActionResult> DeleteRadiologyResult(Guid id)
        {
            var success = await radiologyResultService.DeleteRadiologyResultAsync(id);
            var response = new Response<bool>(success)
            {
                Succeeded = success,
                StatusCode = success ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.BadRequest,
                Message = success ? "Radiology result and associated images deleted successfully." : "Failed to delete radiology result."
            };
            return NewResult(response);
        }
    }
}