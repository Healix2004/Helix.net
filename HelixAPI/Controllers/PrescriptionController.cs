using Helix.Api.Base;
using Helix.Core.Bases;
using Helix.Data.Enums;
using Helix.Service.DTOs.MedicalHistoryDtos;
using Helix.Service.DTOs.PrescriptionDtos;
using Helix.Service.Helper;
using Helix.Service.Interfaces; // ADDED: Need this to inject IDoctorService
using Helix.Service.Services.ConsentService;
using Helix.Service.Services.DoctorService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    [Route("api/prescriptions")]
    [ApiController]
    public class PrescriptionController(IPrescriptionService prescriptionService,IDoctorService doctorService) : AppControllerBase
    {
        // ==========================================
        // CLINICAL SUMMARY (Sidebar View)
        // ==========================================

        [HttpGet("patient/{patientId}/summary")]
        [Authorize(Roles = nameof(EnRoles.Doctor))]
        public async Task<IActionResult> GetClinicalSummary(Guid patientId)
        {
            // Note: Frontend must send X-Consent-Token header
            var summary = await prescriptionService.GetPatientClinicalSummaryAsync(patientId);

            if (summary == null) return NotFound("Patient data not found.");

            return NewResult(new Response<PatientClinicalSummaryDto>(summary)
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "Clinical summary retrieved successfully."
            });
        }

        // ==========================================
        // PRESCRIPTION MANAGEMENT
        // ==========================================
        [HttpPost]
        [Authorize(Roles = nameof(EnRoles.Doctor))]
        public async Task<IActionResult> AddPrescription([FromBody] PrescriptionPayloadDto dto)
        {
            // 1. Security: Extract current doctor context
            var doctorId = await User.GetDoctorIdAsync(doctorService);

            if (doctorId == Guid.Empty)
            {
                return Unauthorized(new { Message = "Doctor context could not be verified." });
            }
            // 2. Execute the unified creation service
            var prescriptionId = await prescriptionService.CreatePrescriptionAsync(doctorId, dto);
            // 3. Return success response to the frontend
            return Ok(new
            {
                Message = "Prescription and diagnostic orders saved successfully.",
                PrescriptionId = prescriptionId
            });
        }

        [HttpPut("item/{itemId}/status")]
        [Authorize(Roles = $"{nameof(EnRoles.Doctor)},{nameof(EnRoles.Admin)}")]
        public async Task<IActionResult> UpdateMedicationStatus(Guid itemId, [FromQuery] EnPrescriptionItemStatus newStatus)
        {
            var success = await prescriptionService.UpdatePrescriptionItemStatusAsync(itemId, newStatus);

            return NewResult(new Response<bool>(success)
            {
                Succeeded = success,
                StatusCode = success ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.NotFound,
                Message = success ? $"Medication status updated to {newStatus}." : "Medication item not found."
            });
        }
    }
}