using Helix.Api.Base;
using Helix.Core.Bases; // ADDED: For manual Response<T> wrappers
using Helix.Core.Features.Allergies.Commands.Models;
using Helix.Core.Features.Allergies.Queries.Models;
using Helix.Data.Enums;
using Helix.Service.DTOs.AllergyDTOs;
using Helix.Service.Helper;
using Helix.Service.Interfaces; // ADDED: For IPatientService
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    [Route("api/allergies")] // FIX 1: Explicit RESTful routing
    [ApiController]
    public class AllergyController(IMediator mediator, IPatientService patientService, IDoctorService doctorService, IEmergencyAccessService emergencyAccessService) : AppControllerBase
    {
        // ==========================================
        // 1. PATIENT WORKFLOW
        // ==========================================

        [HttpGet("my-allergies")]
        [Authorize(Roles = nameof(EnRoles.Patient))]
        [ProducesResponseType(typeof(IEnumerable<AllergyDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyAllergies()
        {
            // Safely extract the ID from the JWT without IDOR risk
            Guid patientId = await User.GetPatientIdAsync(patientService);

            // Note: Make sure you have created this specific Query in your MediatR features!
            var query = new GetAllergyListForPatientQuery(patientId);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        // ==========================================
        // 2. DOCTOR / CLINICAL WORKFLOW (Requires Consent to Read)
        // ==========================================

        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = nameof(EnRoles.Doctor))] // Nurses frequently record and check allergies too!
        [ProducesResponseType(typeof(IEnumerable<AllergyDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPatientAllergies(Guid patientId)
        {
            // 1. Check standard QR Consent
            bool hasStandardConsent = User.HasValidConsent(patientId, "Allergies");

            var doctorId = await User.GetDoctorIdAsync(doctorService);
            // 2. Check Emergency "Break the Glass" Consent
            bool hasEmergencyConsent = await emergencyAccessService.HasActiveEmergencyAccessAsync(doctorId, patientId);

            if (!hasStandardConsent && !hasEmergencyConsent)
            {
                var forbiddenResponse = new Helix.Core.Bases.Response<bool>(false)
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    Message = "You do not have active consent to view this patient's allergy records."
                };
                return NewResult(forbiddenResponse);
            }

            var query = new GetAllergyListForPatientQuery(patientId);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpPost]
        [Authorize(Roles = "Doctor,Nurse")] // Clinicians record allergies
        [ProducesResponseType(typeof(AllergyDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateAllergyDto dto)
        {
            var command = new CreateAllergyCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor,Admin")]
        [ProducesResponseType(typeof(AllergyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAllergyDto dto)
        {
            // FIX 2: Security check to prevent ID spoofing in the JSON body
            if (id != dto.Id)
            {
                var badResponse = new Response<bool>(false)
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = "The ID in the URL does not match the ID in the body."
                };
                return NewResult(badResponse);
            }

            var command = new UpdateAllergyCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        // ==========================================
        // 3. ADMIN WORKFLOW (System Management)
        // ==========================================

        [HttpGet("all")] // RESTful path
        [Authorize(Roles = nameof(EnRoles.Admin))]
        [ProducesResponseType(typeof(IEnumerable<AllergyDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllergyListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Doctor")] // FIX 3: Closed the public data breach!
        [ProducesResponseType(typeof(AllergyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetAllergyByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(EnRoles.Admin))] // Doctors may need to remove an incorrectly logged allergy
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteAllergyCommand(id);
            var response = await mediator.Send(command);
            return NewResult(response);
        }
    }
}