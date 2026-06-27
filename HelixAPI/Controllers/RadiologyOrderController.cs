using Helix.Api.Base;
using Helix.Core.Bases;
using Helix.Data.Enums;
using Helix.Service.DTOs.RadiologyOrderDto;
using Helix.Service.DTOs.RadiologyReportDtos;
using Helix.Service.DTOs.RadiologyTestResultDto;
using Helix.Service.Helper;
using Helix.Service.Interfaces;
using Helix.Service.Services; // Ensure this is imported for IConsentValidationService
using Helix.Service.Services.EmergencyAccessService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    [Route("api/radiology-orders")]
    [ApiController]
    public class RadiologyOrderController(
        IRadiologyOrderService radiologyOrderService,
        IPatientService patientService,
        IDoctorService doctorService,
        IConsentValidationService consentValidationService, // FIXED: Added missing injection
        IEmergencyAccessService emergencyAccessService // FIXED: Added for emergency access
        ) : AppControllerBase
    {
        // ==========================================
        // PATIENT WORKFLOW
        // ==========================================

        [HttpGet("my-pending")]
        [Authorize(Roles = nameof(EnRoles.Patient))]
        public async Task<IActionResult> GetPendingOrders()
        {
            var patientId = await User.GetPatientIdAsync(patientService);
            var result = await radiologyOrderService.GetPendingOrdersAsync(patientId);

            var response = new Response<List<PendingRadiologyOrderDto>>(result)
            {
                Succeeded = result != null,
                StatusCode = result != null ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.NotFound,
                Message = result != null ? "Pending radiology orders retrieved successfully." : "No pending radiology orders found."
            };
            return NewResult(response);
        }

        // ==========================================
        // RADIOLOGY SPECIALIST WORKFLOW (QR)
        // ==========================================

        [HttpGet("scan/{qrToken}")]
        [Authorize(Roles = "Admin,Radiologist")]
        public async Task<IActionResult> ScanRadiologyOrder(string qrToken)
        {
            var result = await radiologyOrderService.ScanRadiologyOrderAsync(qrToken);

            var response = new Response<RadiologyOrderDto>(result)
            {
                Succeeded = result != null,
                StatusCode = result != null ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.NotFound,
                Message = result != null ? "Radiology order scanned successfully." : "Invalid QR token or radiology order not found."
            };
            return NewResult(response);
        }

        [HttpPost("{orderId}/results")]
        [Authorize(Roles = "Admin,Radiologist")]
        public async Task<IActionResult> UploadRadiologyResult(Guid orderId, [FromForm] CreateRadiologyTestResultDto dto)
        {
            dto.OrderId = orderId;
            var result = await radiologyOrderService.UploadResultAsync(dto);

            var response = new Response<bool>(result)
            {
                Succeeded = result,
                StatusCode = result ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.BadRequest,
                Message = result ? "Radiology result uploaded successfully." : "Failed to upload radiology result."
            };
            return NewResult(response);
        }

        // ==========================================
        // DOCTOR WORKFLOW
        // ==========================================

        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = nameof(EnRoles.Doctor))]
        public async Task<IActionResult> GetPatientRadiologyOrders(Guid patientId) // FIXED: Renamed method
        {
            bool hasStandardConsent = false;
            var doctorId = await User.GetDoctorIdAsync(doctorService);

            // 1. Extract the Custom Header
            if (Request.Headers.TryGetValue("X-Consent-Token", out var consentTokenString))
            {
                // 2. Validate the Cryptographic Signature
                var consentPrincipal = consentValidationService.GetPrincipalFromConsentToken(consentTokenString);

                if (consentPrincipal != null)
                {
                    // FIXED: Checked for "Radiology" scope instead of "Labs", and enforced doctorId
                    hasStandardConsent = consentPrincipal.HasValidConsent(patientId, "Radiology", doctorId);
                }
            }

            // 3. Check Emergency "Break the Glass" Consent
            bool hasEmergencyConsent = await emergencyAccessService.HasActiveEmergencyAccessAsync(doctorId, patientId);

            // The Gateway Check
            if (!hasStandardConsent && !hasEmergencyConsent)
            {
                return NewResult(new Response<bool>(false)
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    Message = "You do not have a valid, active consent token to view this patient's Radiology orders."
                });
            }

            // 4. Fetch the data if authorized
            var result = await radiologyOrderService.GetOrdersByPatientAsync(patientId); // FIXED: Uses correct service

            return NewResult(new Response<List<RadiologyOrderDto>>(result) // FIXED: Uses correct DTO
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = result.Count > 0 ? "Order history retrieved successfully." : "No orders found for this patient."
            });
        }

        [HttpGet("my-orders")]
        [Authorize(Roles = nameof(EnRoles.Doctor))]
        public async Task<IActionResult> GetOrdersByDoctor()
        {
            var doctorId = await User.GetDoctorIdAsync(doctorService);
            var result = await radiologyOrderService.GetOrdersByDoctorAsync(doctorId);

            var response = new Response<List<RadiologyOrderDto>>(result)
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = result.Count > 0 ? "Radiology orders retrieved successfully." : "No radiology orders found for this doctor."
            };
            return NewResult(response);
        }

        [HttpPost]
        [Authorize(Roles = nameof(EnRoles.Doctor))]
        public async Task<IActionResult> CreateRadiologyOrder([FromBody] CreateRadiologyOrderDto dto)
        {
            dto.DoctorId = await User.GetDoctorIdAsync(doctorService);
            var result = await radiologyOrderService.CreateRadiologyOrderAsync(dto);

            var response = new Response<Guid>(result)
            {
                StatusCode = result != Guid.Empty ? System.Net.HttpStatusCode.Created : System.Net.HttpStatusCode.BadRequest,
                Succeeded = result != Guid.Empty,
                Message = result != Guid.Empty ? "Radiology order created successfully." : "Failed to create radiology order."
            };
            return NewResult(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = nameof(EnRoles.Doctor))]
        public async Task<IActionResult> UpdateRadiologyOrder(Guid id, [FromBody] UpdateRadiologyOrderDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("The ID in the URL does not match the ID in the body.");
            }

            var result = await radiologyOrderService.UpdateRadiologyOrderAsync(id, dto);

            var response = new Response<bool>(result)
            {
                Succeeded = result,
                StatusCode = result ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.BadRequest,
                Message = result ? "Order updated successfully." : "Update failed."
            };
            return NewResult(response);
        }

        // ==========================================
        // ADMIN & MANAGEMENT WORKFLOW (CRUD)
        // ==========================================

        [HttpGet("{id}")]
        [Authorize(Roles = nameof(EnRoles.Admin))]
        public async Task<IActionResult> GetRadiologyOrderById(Guid id)
        {
            var result = await radiologyOrderService.GetRadiologyOrderByIdAsync(id);
            var response = new Response<RadiologyOrderDto>(result)
            {
                Succeeded = result != null,
                StatusCode = result != null ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.NotFound,
                Message = result != null ? "Radiology order retrieved successfully." : "Radiology order not found."
            };
            return NewResult(response);
        }

        [HttpGet("all")]
        [Authorize(Roles = nameof(EnRoles.Admin))]
        public async Task<IActionResult> GetAllRadiologyOrders()
        {
            var result = await radiologyOrderService.GetAllRadiologyOrdersAsync();
            var response = new Response<List<RadiologyOrderDto>>(result)
            {
                Succeeded = result != null,
                StatusCode = result != null ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.NotFound,
                Message = result != null && result.Count > 0 ? "Radiology orders retrieved successfully." : "No radiology orders found."
            };
            return NewResult(response);
        }

        [HttpPut("{id}/status/{newStatus}")]
        [Authorize(Roles = "Admin,Radiologist")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, string newStatus)
        {
            var result = await radiologyOrderService.UpdateOrderStatusAsync(id, newStatus);
            var response = new Response<bool>(result)
            {
                Succeeded = result,
                StatusCode = result ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.BadRequest,
                Message = result ? "Radiology order status updated successfully." : "Failed to update radiology order status."
            };
            return NewResult(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(EnRoles.Admin))]
        public async Task<IActionResult> DeleteRadiologyOrder(Guid id)
        {
            var result = await radiologyOrderService.DeleteRadiologyOrderAsync(id);
            var response = new Response<bool>(result)
            {
                Succeeded = result,
                StatusCode = result ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.BadRequest,
                Message = result ? "Radiology order deleted successfully." : "Failed to delete radiology order."
            };
            return NewResult(response);
        }
        [HttpGet("order/{orderId}")]
        [Authorize] // Ensure proper roles/consent are applied here!
        public async Task<IActionResult> GetReportByOrderId(Guid orderId)
        {
            var result = await radiologyOrderService.GetReportByOrderIdAsync(orderId);

            if (result == null)
            {
                return NewResult(new Response<RadiologyReportDto>(null)
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    Message = "No radiology report has been uploaded for this order yet."
                });
            }

            return NewResult(new Response<RadiologyReportDto>(result)
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "Radiology report retrieved successfully."
            });
        }
    }
}