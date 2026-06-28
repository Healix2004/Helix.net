using Helix.Api.Base;
using Helix.Core.Bases;
using Helix.Data.Enums;
using Helix.Service.DTOs.AppointmentDtos;
using Helix.Service.Helper;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    [Route("api/appointments")]
    [ApiController]
    public class AppointmentController(IAppointmentService appointmentService,IDoctorService doctorService,IPatientService patientService) : AppControllerBase
    {
        // ==========================================
        // DASHBOARD WORKFLOW (Your UI Design)
        // ==========================================

        [HttpGet("dashboard/today")]
        [Authorize(Roles = nameof(EnRoles.Doctor))]
        public async Task<IActionResult> GetTodayAppointments()
        {
            // Securely grab the logged-in doctor's ID from their token
            var doctorId = await User.GetDoctorIdAsync(doctorService);

            var result = await appointmentService.GetDoctorAppointmentsForTodayAsync(doctorId);

            var response = new Response<List<AppointmentListDto>>(result)
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = result.Count > 0 ? "Today's schedule retrieved successfully." : "No appointments scheduled for today."
            };
            return NewResult(response);
        }

        [HttpGet("dashboard/summary")]
        [Authorize(Roles = nameof(EnRoles.Doctor))]
        public async Task<IActionResult> GetDailySummary()
        {
            var doctorId = await User.GetDoctorIdAsync(doctorService);

            // This powers the "Daily Schedule Summary" card on the right of your UI
            var result = await appointmentService.GetDailyScheduleSummaryAsync(doctorId);

            var response = new Response<DailyScheduleSummaryDto>(result)
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "Daily summary generated successfully."
            };
            return NewResult(response);
        }

        // ==========================================
        // MANAGEMENT WORKFLOW (CRUD & Status)
        // ==========================================

        [HttpPost]
        [Authorize(Roles = $"{nameof(EnRoles.Doctor)},{nameof(EnRoles.Admin)}")]
        public async Task<IActionResult> ScheduleAppointment([FromBody] CreateAppointmentDto dto)
        {
            // Optional: If a doctor is creating this, force the DoctorId to be their own ID
            // so they can't accidentally schedule patients for other doctors.
            if (User.IsInRole(nameof(EnRoles.Doctor)))
            {
                dto.DoctorId = await User.GetDoctorIdAsync(doctorService);
            }

            var resultId = await appointmentService.ScheduleAppointmentAsync(dto);

            var response = new Response<Guid>(resultId)
            {
                Succeeded = resultId != Guid.Empty,
                StatusCode = resultId != Guid.Empty ? System.Net.HttpStatusCode.Created : System.Net.HttpStatusCode.BadRequest,
                Message = resultId != Guid.Empty ? "Appointment scheduled successfully." : "Failed to schedule appointment."
            };
            return NewResult(response);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = $"{nameof(EnRoles.Doctor)},{nameof(EnRoles.Admin)}")]
        public async Task<IActionResult> UpdateAppointmentStatus(Guid id, [FromQuery] string newStatus)
        {
            // Parse the string sent from the frontend (e.g., "Arrived", "Fulfilled") back into our FHIR Enum
            if (!Enum.TryParse<EnAppointmentStatus>(newStatus, true, out var parsedStatus))
            {
                return NewResult(new Response<bool>(false)
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = $"'{newStatus}' is not a valid appointment status."
                });
            }

            var success = await appointmentService.UpdateAppointmentStatusAsync(id, parsedStatus);

            var response = new Response<bool>(success)
            {
                Succeeded = success,
                StatusCode = success ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.NotFound,
                Message = success ? $"Appointment marked as {parsedStatus}." : "Appointment not found."
            };
            return NewResult(response);
        }
        [HttpGet("doctor/{doctorId}/slots")]
        [Authorize] // Any logged-in user (Patient, Admin, Nurse) should be able to view slots
        public async Task<IActionResult> GetAvailableSlots(Guid doctorId, [FromQuery] DateTime date)
        {
            var slots = await appointmentService.GetAvailableTimeSlotsAsync(doctorId, date);

            var response = new Response<List<TimeSlotDto>>(slots)
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "Time slots retrieved successfully."
            };
            return NewResult(response);
        }
    }
}