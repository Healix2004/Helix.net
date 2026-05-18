using Helix.Api.Base;
using Helix.Core.Bases; // ADDED: For manual Response<T> wrappers
using Helix.Core.Features.Medications.Commands.Models;
using Helix.Core.Features.Medications.Queries.Models;
using Helix.Data.Enums;
using Helix.Service.DTOs.MedicationDTOs;
using Helix.Service.Helper;
using Helix.Service.Interfaces; // ADDED: Need this to inject IPatientService
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    /// <summary>
    /// Manages medication records prescribed to patients in the HELIX healthcare ecosystem.
    /// </summary>
    [Route("api/medications")] // FIX 1: Explicit RESTful routing
    [ApiController]
    public class MedicationController(IMediator mediator, IPatientService patientService, IDoctorService doctorService, IEmergencyAccessService emergencyAccessService) : AppControllerBase
    {
        // ==========================================
        // 1. PATIENT WORKFLOW
        // ==========================================

        [HttpGet("my-medications")]
        [Authorize(Roles = nameof(EnRoles.Patient))]
        [ProducesResponseType(typeof(IEnumerable<MedicationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyMedications()
        {
            // Safely extract the ID from the JWT without IDOR risk
            Guid patientId = await User.GetPatientIdAsync(patientService);

            // Note: Make sure you have created this specific Query in your MediatR features!
            var query = new GetMedicationListForPatientQuery(patientId);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        // ==========================================
        // 2. DOCTOR WORKFLOW (Requires Consent to Read)
        // ==========================================

        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = nameof(EnRoles.Doctor))]
        [ProducesResponseType(typeof(IEnumerable<MedicationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPatientMedications(Guid patientId)
        {
            // 1. Check standard QR Consent
            bool hasStandardConsent = User.HasValidConsent(patientId, "Medications");

            var doctorId = await User.GetDoctorIdAsync(doctorService);
            // 2. Check Emergency "Break the Glass" Consent
            bool hasEmergencyConsent = await emergencyAccessService.HasActiveEmergencyAccessAsync(doctorId, patientId);

            if (!hasStandardConsent && !hasEmergencyConsent)
            {
                var forbiddenResponse = new Helix.Core.Bases.Response<bool>(false)
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    Message = "You do not have active consent to view this patient's clinical Medications."
                };
                return NewResult(forbiddenResponse);
            }

            var query = new GetMedicationListForPatientQuery(patientId);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpPost]
        [Authorize(Roles = nameof(EnRoles.Doctor))] // Only Doctors prescribe medications
        [ProducesResponseType(typeof(MedicationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateMedicationDto dto)
        {
            // Note: If dto requires a DoctorId, you can set it here using: 
            // dto.PrescribingDoctorId = await User.GetDoctorIdAsync(doctorService);

            var command = new CreateMedicationCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor,Admin")] // Doctors alter dosages, Admins fix system records
        [ProducesResponseType(typeof(MedicationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMedicationDto dto)
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

            var command = new UpdateMedicationCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        // ==========================================
        // 3. ADMIN WORKFLOW (System Management)
        // ==========================================

        [HttpGet("all")] // RESTful path
        [Authorize(Roles = nameof(EnRoles.Admin))]
        [ProducesResponseType(typeof(IEnumerable<MedicationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetMedicationListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Doctor")] // FIX 3: Closed the public data breach!
        [ProducesResponseType(typeof(MedicationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetMedicationByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Doctor")] // Doctors might need to delete a mistaken prescription entry
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteMedicationCommand(id);
            var response = await mediator.Send(command);
            return NewResult(response);
        }
    }
}