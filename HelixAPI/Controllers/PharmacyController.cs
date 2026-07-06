using Helix.Api.Base;
using Helix.Core.Bases;
using Helix.Service.DTOs.Pharmacy;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Helix.API.Controllers
{
    [Route("api/pharmacies")]
    [ApiController]
    [Authorize] // Ensures only logged-in users can access this endpoint
    public class PharmacyController : AppControllerBase
    {
        private readonly IPharmacyService _pharmacyService;

        public PharmacyController(IPharmacyService pharmacyService)
        {
            _pharmacyService = pharmacyService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterPharmacy([FromForm] RegisterPharmacyDto dto)
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

                // 2. Call the service to upload files and save to the database
                var pharmacy = await _pharmacyService.RegisterPharmacyAsync(dto, appUserId);

                // 3. Return a successful standardized Helix response
                return NewResult(new Response<string>("Pharmacy registered successfully and is pending admin verification.")
                {
                    Succeeded = true,
                    StatusCode = System.Net.HttpStatusCode.Created
                });
            }
            catch (InvalidOperationException ex)
            {
                // Catches the specific error if the user already has a pharmacy registered
                return NewResult(new Response<string>(ex.Message)
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }
            catch (Exception ex)
            {
                // Fallback for unexpected errors (e.g., file system permission issues)
                return NewResult(new Response<string>("An error occurred while processing your registration.")
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Errors = new System.Collections.Generic.List<string> { ex.Message }
                });
            }
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            // Extract the AppUserId from the ClaimsPrincipal
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid appUserId))
            {
                return NewResult(new Response<PharmacyDashboardDto>("User profile could not be verified.")
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                });
            }

            // Fetch the aggregated dashboard data
            var dashboardData = await _pharmacyService.GetDashboardAsync(appUserId);

            if (dashboardData == null)
            {
                return NewResult(new Response<PharmacyDashboardDto>("No approved pharmacy profile found for this user.")
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                });
            }

            return NewResult(new Response<PharmacyDashboardDto>(dashboardData)
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "Pharmacy dashboard retrieved successfully."
            });
        }

        [HttpGet("prescriptions/{prescriptionId:guid}")]
        [Authorize(Roles = "Pharmacist")]
        public async Task<IActionResult> GetPrescriptionDetails([FromRoute] Guid prescriptionId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid appUserId))
                return Unauthorized();

            var data = await _pharmacyService.GetPrescriptionDetailsForPharmacyAsync(prescriptionId, appUserId);

            if (data == null)
            {
                return NewResult(new Response<PharmacyPrescriptionDetailsDto>("Prescription not found or access denied.")
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                });
            }

            return NewResult(new Response<PharmacyPrescriptionDetailsDto>(data) { Succeeded = true });
        }

        [HttpPost("prescriptions/{prescriptionId:guid}/dispense")]
        [Authorize(Roles = "Pharmacist")]
        public async Task<IActionResult> DispensePrescription([FromRoute] Guid prescriptionId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid appUserId))
                return Unauthorized();

            var success = await _pharmacyService.DispensePrescriptionAsync(prescriptionId, appUserId);

            if (!success)
                return NewResult(new Response<string>("Failed to dispense. Prescription may already be dispensed or not found.") { Succeeded = false, StatusCode = System.Net.HttpStatusCode.BadRequest });

            return NewResult(new Response<string>("Prescription dispensed successfully.") { Succeeded = true });
        }

        [HttpPost("prescriptions/{prescriptionId:guid}/flag")]
        [Authorize(Roles = "Pharmacist")]
        public async Task<IActionResult> FlagPrescription([FromRoute] Guid prescriptionId, [FromBody] FlagPrescriptionDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid appUserId))
                return Unauthorized();

            var success = await _pharmacyService.FlagPrescriptionAsync(prescriptionId, dto, appUserId);

            if (!success)
                return NewResult(new Response<string>("Failed to flag prescription.") { Succeeded = false, StatusCode = System.Net.HttpStatusCode.BadRequest });

            return NewResult(new Response<string>("Prescription flagged for review by the prescribing doctor.") { Succeeded = true });
        }
    }
}