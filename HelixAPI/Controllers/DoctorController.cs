using Helix.Api.Base;
using Helix.Core.Bases;
using Helix.Core.Features.Doctors.Commands.Models;
using Helix.Core.Features.Doctors.Queries.Models;
using Helix.Data.Enums;
using Helix.Service.DTOs.AppointmentDtos;
using Helix.Service.DTOs.DoctorDTOs;
using Helix.Service.Helper;
using Helix.Service.Interfaces; // ADDED: Need this to inject IDoctorService
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace Helix.API.Controllers
{
    [Route("api/doctors")] // FIX 1: Explicit RESTful routing
    [ApiController]
    public class DoctorController(IMediator mediator, IDoctorService doctorService,IAppointmentService appointmentService) : AppControllerBase
    {

        [HttpPost("register-doctor")]
        public async Task<IActionResult> RegisterDoctor([FromForm] RegisterDoctorDto dto)
        {
            // 1. Manually deserialize the JSON strings back into C# Lists
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var timeSlotsList = dto.AvailableTimeSlotsJson.Select(t => JsonSerializer.Deserialize<CreateAvailableTimeSlotDto>(t,options)).ToList();

            // 2. Combine into your actual command DTO
            var createDoctorDto = new CreateDoctorDto
            {
                AppUserId = dto.AppUserId,
                SpecialtyCatalogCode = dto.SpecialtyCatalogCode,
                MedicalLicenseNumber = dto.MedicalLicenseNumber,
                MedicalLicenseDocument = dto.MedicalLicenseDocument,
                NationalIdDocument = dto.NationalIdDocument,
                ProfileImage = dto.ProfileImage,
                FullName= dto.FullName,
                NationalId= dto.NationalId,
                PhoneNumber = dto.PhoneNumber,
                Country = dto.Country,
                State = dto.State,
                YearsOfExperience = dto.YearsOfExperience,
                ClinicAddress = dto.ClinicAddress,
                Bio = dto.Bio,
                ConsultationType = dto.ConsultationType,
                ConsultationFee = dto.ConsultationFee,
                AvailabeDays = dto.AvailabeDays,
                AvailableTimeSlots = timeSlotsList
            };
            var command = new CreateDoctorCommand(createDoctorDto);
            var result = await mediator.Send(command);
            return NewResult(result);
        }
        // ==========================================================
        // 1. FOR THE DOCTOR (Fetching their own profile)
        // ==========================================================
        [HttpGet("my-profile")]
        [Authorize(Roles = nameof(EnRoles.Doctor))]
        [ProducesResponseType(typeof(DoctorDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyProfile()
        {
           var doctorId = await User.GetDoctorIdAsync(doctorService);

            var query = new GetDoctorByIdQuery(doctorId);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        // ==========================================================
        // 2. PUBLIC/SHARED WORKFLOW (Lookup)
        // ==========================================================

        [HttpGet("all")] // RESTful path
        [Authorize] // FIX 2: Anyone logged into HELIX (Admin, Patient, Doctor) can see the list of doctors
        [ProducesResponseType(typeof(IEnumerable<DoctorDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetDoctorListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpGet("{id}")]
        [Authorize] // Anyone logged into HELIX can view a specific doctor's public profile
        [ProducesResponseType(typeof(DoctorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetDoctorByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        // ==========================================
        // ADMIN WORKFLOW (System Management)
        // ==========================================

        [HttpPost]
        [Authorize(Roles = nameof(EnRoles.Admin))] // Explicitly locked back to Admins
        [ProducesResponseType(typeof(DoctorDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateDoctorDto dto)
        {
            var command = new CreateDoctorCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = nameof(EnRoles.Admin))] // (Optional: You could add "Doctor" here if doctors can edit their own profiles)
        [ProducesResponseType(typeof(DoctorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDoctorDto dto)
        {
            // Security check: Prevent ID spoofing
            if (id != dto.Id)
            {
                return BadRequest("The ID in the URL does not match the ID in the body.");
            }

            var command = new UpdateDoctorCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(EnRoles.Admin))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteDoctorCommand(id);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [HttpGet("search/name")]
        public async Task<IActionResult> SearchByName([FromQuery] string name)
        {
            var doctors = await doctorService.SearchDoctorsByNameAsync(name);
            return NewResult(new Response<IEnumerable<DoctorDto>>(doctors)
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "Doctors retrieved successfully."
            });
        }

        [HttpGet("search/specialty")]
        public async Task<IActionResult> SearchBySpecialty([FromQuery] string specialty)
        {
            var doctors = await doctorService.SearchDoctorsBySpecialtyAsync(specialty);
            return NewResult(new Response<IEnumerable<DoctorDto>>(doctors)
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "Doctors retrieved successfully."
            });
        }

        [HttpGet("calendar")]
        public async Task<IActionResult> GetMonthFlags([FromQuery] int year, [FromQuery] int month)
        {
            try
            {
                // Validate input
                if (year < 2000 || month < 1 || month > 12)
                {
                    return NewResult(new Response<CalendarMonthDto>("Invalid year or month parameters.")
                    {
                        Succeeded = false,
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    });
                }

                var doctorId = await User.GetDoctorIdAsync(doctorService);


                var calendarData = await doctorService.GetCalendarFlagsAsync(doctorId, year, month);

                return NewResult(new Response<CalendarMonthDto>(calendarData)
                {
                    Succeeded = true,
                    StatusCode = System.Net.HttpStatusCode.OK
                });
            }
            catch (InvalidOperationException ex)
            {
                return NewResult(new Response<CalendarMonthDto>(ex.Message)
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                });
            }
            catch (Exception ex)
            {
                return NewResult(new Response<CalendarMonthDto>("An error occurred while fetching the calendar.")
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Errors = new System.Collections.Generic.List<string> { ex.Message }
                });
            }
        }

        [HttpGet("appointments/day-date")]
        public async Task<IActionResult> GetDailyAppointments([FromQuery] DateTime date)
        {
            try
            {
                // 1. Basic validation: ensure the date isn't empty (DateTime.MinValue)
                if (date == default)
                {
                    return NewResult(new Response<List<AppointmentListDto>>("A valid date is required. Format: YYYY-MM-DD")
                    {
                        Succeeded = false,
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    });
                }

                // 2. Extract the Doctor's  ID from the JWT token
                var doctorId = await User.GetDoctorIdAsync(doctorService);


                // 3. Fetch the schedule
                var scheduleData = await appointmentService.GetDoctorDayAppointmentAsync(doctorId, date);

                // 4. Return standard Helix response
                return NewResult(new Response<List<AppointmentListDto>>(scheduleData)
                {
                    Succeeded = true,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Message = $"Appointments retrieved for {date:yyyy-MM-dd}"
                });
            }
            catch (InvalidOperationException ex)
            {
                return NewResult(new Response<List<AppointmentListDto>>(ex.Message)
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                });
            }
            catch (Exception ex)
            {
                return NewResult(new Response<List<AppointmentListDto>>("An error occurred while fetching the daily schedule.")
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Errors = new System.Collections.Generic.List<string> { ex.Message }
                });
            }
        }
    }
}