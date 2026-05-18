using Helix.Api.Base;
using Helix.Core.Bases;
using Helix.Data.Enums;
using Helix.Service.DTOs.RadiologyOrderDto;
using Helix.Service.DTOs.RadiologyTestResultDto;
using Helix.Service.Helper;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    [Route("api/radiology-orders")] // FIX 1: Explicit RESTful routing
    [ApiController]
    public class RadiologyOrderController(IRadiologyOrderService radiologyOrderService, IPatientService patientService, IDoctorService doctorService) : AppControllerBase
    {
        // ==========================================
        // PATIENT WORKFLOW
        // ==========================================

        [HttpGet("my-pending")] // FIX 2: Cleaner RESTful route
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
        [Authorize(Roles = "Admin,Radiologist")] // FIX 3: CRITICAL SECURITY LOCK
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
        [Authorize(Roles = "Admin,Radiologist")] // FIX 3: CRITICAL SECURITY LOCK
        // Note: [FromForm] is correct here since we are handling file uploads!
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

        [HttpGet("my-orders")] // FIX 2: Cleaner RESTful route
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

        [HttpPost] // Changed from "create" to standard POST route
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
            // FIX 4: Prevent ID Spoofing!
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
        [Authorize(Roles = "Admin,Radiologist")] // Radiologists likely need to update status too
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
    }
}