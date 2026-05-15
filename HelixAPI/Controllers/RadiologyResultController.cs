using Helix.Api.Base;
using Helix.Core.Bases;
using Helix.Data.Enums;
using Helix.Service.DTOs.RadiologyTestResultDto;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    [Route("api/radiology-results")] // Using explicit RESTful routing
    [ApiController]
    public class RadiologyResultController(IRadiologyResultService radiologyResultService) : AppControllerBase
    {
        // ==========================================
        // PATIENT & DOCTOR WORKFLOW (Retrieving)
        // ==========================================

        [HttpGet("order/{orderId}")]
        // [Authorize(Roles = "Doctor,Patient,Admin")]
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

        [HttpGet("patient/{patientId}")]
        // [Authorize(Roles = "Doctor,Patient,Admin")]
        public async Task<IActionResult> GetResultsByPatientId(Guid patientId)
        {
            var results = await radiologyResultService.GetRadiologyResultsByPatientIdAsync(patientId);
            var response = new Response<List<RadiologyTestResultDto>>(results)
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = results.Count > 0 ? "Patient radiology results retrieved successfully." : "No radiology results found for this patient."
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
        [Authorize(Roles = "Doctor,Admin")] // Assuming Radiologists share the Doctor role
        public async Task<IActionResult> UpdateRadiologyResult(Guid id, [FromBody] UpdateRadiologyTestResultDto dto)
        {
            if (id != dto.Id)
                return BadRequest("The ID in the URL does not match the ID in the body.");

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