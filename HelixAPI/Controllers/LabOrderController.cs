using Helix.Api.Base;
using Helix.Core.Bases;
using Helix.Data.Enums;
using Helix.Service.DTOs.LabOrderDTOs;
using Helix.Service.DTOs.LabTestResultDTOs;
using Helix.Service.Helper;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    [Route("api/lab-orders")] // FIX 1: Explicit RESTful routing
    [ApiController]
    public class LabOrderController(ILabOrderService labOrderService, IPatientService patientService, IDoctorService doctorService) : AppControllerBase
    {
        // ==========================================
        // PATIENT WORKFLOW
        // ==========================================
        [HttpGet("my-pending")] // FIX 2: Cleaner RESTful route
        [Authorize(Roles = nameof(EnRoles.Patient))]
        public async Task<IActionResult> GetPendingOrders()
        {
            var patientId = await User.GetPatientIdAsync(patientService);
            var result = await labOrderService.GetPendingOrdersAsync(patientId);

            var response = new Response<List<PendingLabOrderDto>>(result)
            {
                Succeeded = result != null,
                StatusCode = result != null ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.NotFound,
                Message = result != null ? "Pending lab orders retrieved successfully." : "No pending lab orders found."
            };
            return NewResult(response);
        }

        // ==========================================
        // LAB SPECIALIST WORKFLOW (QR)
        // ==========================================

        [HttpGet("scan/{qrToken}")]
        [Authorize(Roles = nameof(EnRoles.Admin))] // FIX 3: CRITICAL SECURITY LOCK
        public async Task<IActionResult> ScanLabOrder(string qrToken)
        {
            var result = await labOrderService.ScanLabOrderAsync(qrToken);

            var response = new Response<LabOrderDto>(result)
            {
                Succeeded = result != null,
                StatusCode = result != null ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.NotFound,
                Message = result != null ? "Lab order scanned successfully." : "Invalid QR token or lab order not found."
            };
            return NewResult(response);
        }

        [HttpPost("{orderId}/results")]
        [Authorize(Roles = nameof(EnRoles.Patient))]
        public async Task<IActionResult> UploadLabResult(Guid orderId, [FromBody] UploadLabResultDto dto)
        {
            dto.OrderId = orderId;
            var result = await labOrderService.UploadLabResultAsync(dto);

            var response = new Response<bool>(result)
            {
                Succeeded = result,
                StatusCode = result ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.BadRequest,
                Message = result ? "Lab result uploaded successfully." : "Failed to upload lab result."
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
            var result = await labOrderService.GetOrdersByDoctorAsync(doctorId);

            var response = new Response<List<LabOrderDto>>(result)
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = result.Count > 0 ? "Lab orders retrieved successfully." : "No lab orders found for this doctor."
            };
            return NewResult(response);
        }

        [HttpPost] // Changed from "create" to standard POST route
        [Authorize(Roles = nameof(EnRoles.Doctor))]
        public async Task<IActionResult> CreateLabOrder([FromBody] CreateLabOrderDto dto)
        {
            dto.DoctorId = await User.GetDoctorIdAsync(doctorService);
            var result = await labOrderService.CreateLabOrderAsync(dto);

            var response = new Response<Guid>(result) // Note: No need for .ToString() if Response<T> takes a Guid
            {
                StatusCode = result != Guid.Empty ? System.Net.HttpStatusCode.Created : System.Net.HttpStatusCode.BadRequest,
                Succeeded = result != Guid.Empty,
                Message = result != Guid.Empty ? "Lab order created successfully." : "Failed to create lab order."
            };
            return NewResult(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = nameof(EnRoles.Doctor))]
        public async Task<IActionResult> UpdateLabOrder(Guid id, [FromBody] UpdateLabOrderDto dto)
        {
            // FIX 4: Prevent ID Spoofing!
            if (id != dto.Id)
            {
                return BadRequest("The ID in the URL does not match the ID in the body.");
            }

            var result = await labOrderService.UpdateLabOrderAsync(id, dto);
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
        public async Task<IActionResult> GetLabOrderById(Guid id)
        {
            var result = await labOrderService.GetLabOrderByIdAsync(id);
            var response = new Response<LabOrderDto>(result)
            {
                Succeeded = result != null,
                StatusCode = result != null ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.NotFound,
                Message = result != null ? "Lab order retrieved successfully." : "Lab order not found."
            };
            return NewResult(response);
        }

        [HttpGet("all")]
        [Authorize(Roles = nameof(EnRoles.Admin))]
        public async Task<IActionResult> GetAllLabOrders()
        {
            var result = await labOrderService.GetAllLabOrdersAsync();
            var response = new Response<List<LabOrderDto>>(result)
            {
                Succeeded = result != null,
                StatusCode = result != null ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.NotFound,
                Message = result != null && result.Count > 0 ? "Lab orders retrieved successfully." : "No lab orders found."
            };
            return NewResult(response);
        }

        [HttpPut("{id}/status/{newStatus}")]
        [Authorize(Roles = nameof(EnRoles.Patient))]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, string newStatus)
        {
            var result = await labOrderService.UpdateOrderStatusAsync(id, newStatus);
            var response = new Response<bool>(result)
            {
                Succeeded = result,
                StatusCode = result ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.BadRequest,
                Message = result ? "Lab order status updated successfully." : "Failed to update lab order status."
            };
            return NewResult(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(EnRoles.Admin))]
        public async Task<IActionResult> DeleteLabOrder(Guid id)
        {
            var result = await labOrderService.DeleteLabOrderAsync(id);
            var response = new Response<bool>(result)
            {
                Succeeded = result,
                StatusCode = result ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.BadRequest,
                Message = result ? "Lab order deleted successfully." : "Failed to delete lab order."
            };
            return NewResult(response);
        }
    }
}