using Helix.Api.Base;
using Helix.Core.Bases;
using Helix.Core.Features.Auth.Commands.Models;
using Helix.Core.Features.Patients.Commands.Models;
using Helix.Core.Features.Patients.Queries.Models;
using Helix.Data.Enums;
using Helix.Service.DTOs.AuthDTOs;
using Helix.Service.DTOs.PatientDTOs;
using Helix.Service.Helper;
using Helix.Service.Interfaces; // ADDED: Need this to inject IPatientService
using Helix.Service.Services.DoctorService;
using Helix.Service.Services.EmergencyAccessService;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    /// <summary>
    /// Manages patient records in the HELIX healthcare ecosystem.
    /// </summary>
    [Route("api/patients")] // FIX 1: Explicit RESTful routing
    [ApiController]
    public class PatientController(IMediator mediator, IPatientService patientService, IDoctorService doctorService, IEmergencyAccessService emergencyAccessService) : AppControllerBase
    {
        [HttpPost("register-patient")]
        public async Task<IActionResult> RegisterPatient([FromForm] CreatePatientDto dto)
        {
            var command = new CreatePatientCommand(dto);
            var result = await mediator.Send(command);

            return NewResult(result);
        }
        // ==========================================================
        // 1. FOR THE PATIENT (Fetching their own profile)
        // ==========================================================
        [HttpGet("my-profile")]
        [Authorize(Roles = nameof(EnRoles.Patient))]
        [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyProfile()
        {
            // Safely get the ID from their JWT login token
            Guid patientId = await User.GetPatientIdAsync(patientService);

            var query = new GetPatientByIdQuery(patientId);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        // ==========================================================
        // 2. FOR DOCTORS & ADMINS (Fetching a specific patient)
        // ==========================================================
        [HttpGet("{id}")]
        [Authorize(Roles = $"{nameof(EnRoles.Admin)},{nameof(EnRoles.Doctor)}")]
        [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            // 1. Check standard QR Consent
            bool hasStandardConsent = User.HasValidConsent(id, "Demographics");

            var doctorId = await User.GetDoctorIdAsync(doctorService);
            // 2. Check Emergency "Break the Glass" Consent
            bool hasEmergencyConsent = await emergencyAccessService.HasActiveEmergencyAccessAsync(doctorId, id);

            if (!hasStandardConsent && !hasEmergencyConsent)
            {
                var forbiddenResponse = new Helix.Core.Bases.Response<bool>(false)
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    Message = "You do not have active consent to view this patient's profile."
                };
                return NewResult(forbiddenResponse);
            }

            // Admins bypass the consent check automatically
            var query = new GetPatientByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        // ==========================================
        // ADMIN WORKFLOW (System Management)
        // ==========================================

        [HttpGet("all")] // FIX 3: RESTful path
        [Authorize(Roles = nameof(EnRoles.Admin))]
        [ProducesResponseType(typeof(IEnumerable<PatientDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetPatientListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpPost]
        [Authorize(Roles = nameof(EnRoles.Admin))] // Explicitly locked to Admins
        [ProducesResponseType(typeof(PatientDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreatePatientDto dto)
        {
            var command = new CreatePatientCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = nameof(EnRoles.Admin))]
        [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePatientDto dto)
        {
            // Security check: Prevent ID spoofing in the body
            if (id != dto.Id)
            {
                return BadRequest("The ID in the URL does not match the ID in the body.");
            }

            var command = new UpdatePatientCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(EnRoles.Admin))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeletePatientCommand(id);
            var response = await mediator.Send(command);
            return NewResult(response);
        }
    }    
}