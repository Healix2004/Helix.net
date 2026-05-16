using Helix.Api.Base;
using Helix.Core.Bases;
using Helix.Data.Enums;
using Helix.Service.DTOs.LabTestResultDTOs;
using Helix.Service.DTOs.RadiologyOrderDto;
using Helix.Service.DTOs.RadiologyTestResultDto;
using Helix.Service.Interfaces;
using Helix.Service.Services.LabTestResultService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Helix.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RadiologyOrderController(IRadiologyOrderService radiologyOrderService, IPatientService patientService, IDoctorService doctorService) : AppControllerBase
    {
        // ==========================================
        // PATIENT & RADIOLOGY SPECIALIST WORKFLOW (QR)
        // ==========================================
        [Authorize(Roles = nameof(EnRoles.Patient))]
        [HttpGet("patient/GetPendingOrders")]
        public async Task<IActionResult> GetPendingOrders()
        {
            var patientId = GetPatientId();
            var result = await radiologyOrderService.GetPendingOrdersAsync(patientId);
            var response = new Response<List<PendingRadiologyOrderDto>>(result)
            {
                Succeeded = result != null,
                StatusCode = result != null ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.NotFound,
                Message = result != null ? "Pending radiology orders retrieved successfully." : "No pending radiology orders found."
            };
            return NewResult(response);
        }

        [HttpGet("scan/{qrToken}")]
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
        [Authorize(Roles = nameof(EnRoles.Doctor))]
        [HttpGet("doctor/all")]
        public async Task<IActionResult> GetOrdersByDoctor()
        {
            var doctorId = GetDoctorId();
            var result = await radiologyOrderService.GetOrdersByDoctorAsync(doctorId);
            var response = new Response<List<RadiologyOrderDto>>(result)
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = result.Count > 0 ? "Radiology orders retrieved successfully." : "No radiology orders found for this doctor."
            };
            return NewResult(response);
        }

        [Authorize(Roles = nameof(EnRoles.Doctor))]
        [HttpPost("create")]
        public async Task<IActionResult> CreateRadiologyOrder([FromBody] CreateRadiologyOrderDto dto)
        {
            dto.DoctorId = GetDoctorId();
            var result = await radiologyOrderService.CreateRadiologyOrderAsync(dto);
            var response = new Response<Guid>(result.ToString())
            {
                StatusCode = result != Guid.Empty ? System.Net.HttpStatusCode.Created : System.Net.HttpStatusCode.BadRequest,
                Succeeded = result != Guid.Empty,
                Message = result != Guid.Empty ? "Radiology order created successfully." : "Failed to create radiology order."
            };
            return NewResult(response);
        }

        [Authorize(Roles = nameof(EnRoles.Doctor))]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRadiologyOrder(Guid id, [FromBody] UpdateRadiologyOrderDto dto)
        {
            var result = await radiologyOrderService.UpdateRadiologyOrderAsync(id, dto);
            var response = new Response<bool>(result);
            return NewResult(response);
        }

        // ==========================================
        // ADMIN & MANAGEMENT WORKFLOW (CRUD)
        // ==========================================

        [Authorize(Roles = nameof(EnRoles.Admin))]
        [HttpGet("{id}")]
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

        [Authorize(Roles = nameof(EnRoles.Admin))]
        [HttpGet("all")]
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

        [Authorize(Roles = nameof(EnRoles.Admin))]
        [HttpPut("{id}/status/{newStatus}")]
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

        [Authorize(Roles = nameof(EnRoles.Admin))]
        [HttpDelete("{id}")]
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