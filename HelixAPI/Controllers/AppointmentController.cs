using Helix.Api.Base;
using Helix.Core.Bases;
using Helix.Data.Enums;
using Helix.Service.DTOs.AppointmentDtos;
using Helix.Service.DTOs.DoctorDTOs;
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

        // ==========================================
        // MANAGEMENT WORKFLOW (CRUD & Status)
        // ==========================================

        [HttpPost]
        [Authorize(Roles = $"{nameof(EnRoles.Doctor)},{nameof(EnRoles.Patient)},{nameof(EnRoles.Admin)}")]
        public async Task<IActionResult> ScheduleAppointment([FromBody] CreateAppointmentDto dto)
        {
            // Optional: If a doctor is creating this, force the DoctorId to be their own ID
            // so they can't accidentally schedule patients for other doctors.
            if (User.IsInRole(nameof(EnRoles.Doctor)))
            {
                dto.DoctorId = await User.GetDoctorIdAsync(doctorService);
            }
            else if (User.IsInRole(nameof(EnRoles.Patient)))
            {
                dto.PatientId = await User.GetPatientIdAsync(patientService);
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
            var availability = await appointmentService.GetAvailableTimeSlotsAsync(doctorId, date);

            var response = new Response<DoctorAvailabilityDto>(availability)
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "Availability retrieved successfully."
            };
            return NewResult(response);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(Response<IEnumerable<DoctorSearchResultDto>>), StatusCodes.Status200OK)]
        [Authorize(Roles = $"{nameof(EnRoles.Patient)},{nameof(EnRoles.Doctor)}")]
        public async Task<IActionResult> SearchDoctors([FromQuery] string query, [FromQuery] int count = 20)
        {
            // 1. Call your highly optimized service function
            var doctors = await appointmentService.SearchDoctorsAsync(query, count);

            // 2. Wrap the result in your standard Helix response format
            var response = new Response<IEnumerable<DoctorSearchResultDto>>(doctors)
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "Search completed successfully."
            };

            // 3. Return using your base controller's NewResult method
            return NewResult(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Response<DoctorDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<DoctorDetailsDto>), StatusCodes.Status404NotFound)]
        [Authorize(Roles = $"{nameof(EnRoles.Patient)},{nameof(EnRoles.Doctor)}")]
        public async Task<IActionResult> GetDoctorDetails(Guid id)
        {
            // 1. Fetch the doctor details from the service
            var doctor = await appointmentService.GetDoctorDetailsAsync(id);

            // 2. Handle the Not Found scenario gracefully
            if (doctor == null)
            {
                return NewResult(new Response<DoctorDetailsDto>
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    Message = "Doctor not found."
                });
            }

            // 3. Return the successful result
            return NewResult(new Response<DoctorDetailsDto>(doctor)
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "Doctor details retrieved successfully."
            });
        }
        // ==========================================
        // SCHEDULE MODAL WORKFLOW (Calendar & Slots)
        // ==========================================
        [HttpGet("doctor/{doctorId}/available-days")]
        [Authorize(Roles = $"{nameof(EnRoles.Patient)},{nameof(EnRoles.Doctor)},{nameof(EnRoles.Admin)}")]
        public async Task<IActionResult> GetAvailableDaysInMonth(Guid doctorId, [FromQuery] int year, [FromQuery] int month)
        {
            if (month < 1 || month > 12)
                return NewResult(new Response<AvailableDaysDto> { Succeeded = false, StatusCode = System.Net.HttpStatusCode.BadRequest, Message = "Invalid month." });

            Guid? patientId = null;

            // Extract the Patient ID if a patient is the one viewing the calendar
            if (User.IsInRole(nameof(EnRoles.Patient)))
            {
                patientId = await User.GetPatientIdAsync(patientService);
            }

            var result = await appointmentService.GetAvailableDaysInMonthAsync(doctorId, year, month, patientId);

            return NewResult(new Response<AvailableDaysDto>(result)
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "Available days retrieved successfully."
            });
        }
        [HttpGet("doctor/{doctorId}/available-slots")]
        [Authorize(Roles = $"{nameof(EnRoles.Patient)},{nameof(EnRoles.Doctor)},{nameof(EnRoles.Admin)}")]
        public async Task<IActionResult> GetTimeSlotsForDay(Guid doctorId, [FromQuery] DateTime date)
        {
            // Ensure we are only looking at the date part, stripping any time data sent by the frontend
            var cleanDate = date.Date;
            Guid? patientId = User.IsInRole(nameof(EnRoles.Patient))
                ? (Guid?)(await User.GetPatientIdAsync(patientService))
                : null;
            var result = await appointmentService.GetTimeSlotsForDayAsync(doctorId, cleanDate, patientId);

            return NewResult(new Response<DayTimeSlotsDto>(result)
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "Time slots retrieved successfully."
            });
        }
        // ==========================================
        // DOCTOR DASHBOARD WORKFLOW
        // ==========================================

        [HttpGet("dashboard/list")]
        [Authorize(Roles = nameof(EnRoles.Doctor))]
        public async Task<IActionResult> GetDashboardList([FromQuery] string filter = "today")
        {
            var doctorId = await User.GetDoctorIdAsync(doctorService); // Your existing extension method
            var result = await appointmentService.GetDoctorAppointmentsAsync(doctorId, filter);

            return NewResult(new Response<List<AppointmentListDto>>(result)
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = $"Appointments for '{filter}' retrieved successfully."
            });
        }
        [HttpGet("dashboard/high-priority")]
        [Authorize(Roles = nameof(EnRoles.Doctor))]
        public async Task<IActionResult> GetHighPriorityPatients()
        {
            var doctorId = await User.GetDoctorIdAsync(doctorService);
            var result = await appointmentService.GetHighPriorityPatientsTodayAsync(doctorId);

            return NewResult(new Response<List<AppointmentListDto>>(result)
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "High-priority patients retrieved successfully."
            });
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

    }
}