using Helix.Api.Base;
using Helix.Core.Bases;
using Helix.Data.Enums;
using Helix.Service.DTOs.LabOrderDTOs;
using Helix.Service.DTOs.LabTestResultDTOs;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Helix.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabOrderController(ILabOrderService labOrderService, IPatientService patientService, IDoctorService doctorService) : AppControllerBase
    {
        // ==========================================
        // PATIENT & LAB SPECIALIST WORKFLOW (QR)
        // ==========================================
        [Authorize(Roles = nameof(EnRoles.Patient))]
        [HttpGet("patient/GetPendingOrders")]
        public async Task<IActionResult> GetPendingOrders()
        {
            var patientId = GetPatientId();
            var result = await labOrderService.GetPendingOrdersAsync(patientId);
            var response = new Response<List<PendingLabOrderDto>>(result)
            {
                Succeeded = result != null,
                StatusCode = result != null ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.NotFound,
                Message = result != null ? "Pending lab orders retrieved successfully." : "No pending lab orders found."
            };
            return NewResult(response);
        }
        [HttpGet("scan/{qrToken}")]
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
        public async Task<IActionResult> UploadLabResult(Guid orderId, [FromBody] UploadLabResultDto dto)
        {
            dto.OrderId = orderId; // <-- Check this property name
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
        [Authorize(Roles = nameof(EnRoles.Doctor))]
        [HttpGet("doctor/all")]
        public async Task<IActionResult> GetOrdersByDoctor()
        {
            var doctorId = GetDoctorId();
            var result = await labOrderService.GetOrdersByDoctorAsync(doctorId);
            var response = new Response<List<LabOrderDto>>(result)
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = result.Count > 0 ? "Lab orders retrieved successfully." : "No lab orders found for this doctor."
            };
            return NewResult(response);
        }
        [Authorize(Roles = nameof(EnRoles.Doctor))]
        [HttpPost("create")]
        public async Task<IActionResult> CreateLabOrder([FromBody] CreateLabOrderDto dto)
        {
            dto.DoctorId = GetDoctorId();
            var result = await labOrderService.CreateLabOrderAsync(dto);
            var response = new Response<Guid>(result.ToString())
            {
                StatusCode = result != Guid.Empty ? System.Net.HttpStatusCode.Created : System.Net.HttpStatusCode.BadRequest,
                Succeeded = result != Guid.Empty,
                Message = result != Guid.Empty ? "Lab order created successfully." : "Failed to create lab order."
            };
            return NewResult(response);
        }
        [Authorize(Roles = nameof(EnRoles.Doctor))]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLabOrder(Guid id, [FromBody] UpdateLabOrderDto dto)
        {
            var result = await labOrderService.UpdateLabOrderAsync(id, dto);
            var response = new Response<bool>(result);
            return NewResult(response);
        }
        // ==========================================
        // ADMIN & MANAGEMENT WORKFLOW (CRUD)
        // ==========================================

        [Authorize(Roles = nameof(EnRoles.Admin))]
        [HttpGet("{id}")]
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
        [Authorize(Roles = nameof(EnRoles.Admin))]
        [HttpGet("all")]
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
        [Authorize(Roles = nameof(EnRoles.Admin))]
        [HttpPut("{id}/status/{newStatus}")]
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
        [Authorize(Roles = nameof(EnRoles.Admin))]
        [HttpDelete("{id}")]
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

        #region helper 
        private Guid GetPatientId()
        {
            // 1. Extract the user ID from the JWT Claims
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // 2. get the patient record based on the user ID
            var patient = patientService.GetPatientByUserIdAsync(userIdString).Result;
            if (patient != null)
            {
                return patient.Id;
            }

            throw new UnauthorizedAccessException("Invalid patient ID.");
        }
        private Guid GetDoctorId()
        {
            // 1. Extract the user ID from the JWT Claims
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctor = doctorService.GetDoctorByUserIdAsync(userIdString).Result;
            if (doctor != null)
            {
                return doctor.Id;
            }
            throw new UnauthorizedAccessException("Invalid doctor ID.");
        }
        #endregion
    }
}
