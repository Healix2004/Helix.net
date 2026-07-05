using Helix.Api.Base;
using Helix.Core.Bases;
using Helix.Data.Enums;
using Helix.Service.DTOs.MedicalHistoryDtos;
using Helix.Service.Helper;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    [Route("api/patients")]
    [ApiController]
    public class MedicalHistoryController(IMedicalHistoryService medicalHistoryService, IDoctorService doctorService,
        IConsentValidationService consentValidationService, IPatientService patientService,
        IEmergencyAccessService emergencyAccessService) : AppControllerBase
    {

        /// <summary>
        /// Retrieves the comprehensive medical history timeline for a specific patient.
        /// </summary>
        /// <param name="patientId">The unique GUID of the patient.</param>
        /// <returns>A chronological list of medical events.</returns>
        [HttpGet("{patientId:guid}/timeline")]
        [Authorize(Roles = nameof(EnRoles.Doctor))]
        public async Task<IActionResult> GetPatientTimeline([FromRoute] Guid patientId)
        {
            // 1. Get logged-in doctor and validate they exist
            var doctorId = await User.GetDoctorIdAsync(doctorService);

            if (doctorId == Guid.Empty)
            {
                return NewResult(new Response<List<TimelineEventDto>>("Doctor context could not be verified.")
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                });
            }

            // 2. Validate Standard Consent
            bool hasStandardConsent = false;
            if (Request.Headers.TryGetValue("X-Consent-Token", out var consentTokenString))
            {
                var consentPrincipal = consentValidationService.GetPrincipalFromConsentToken(consentTokenString);

                if (consentPrincipal != null)
                {
                    // Security Best Practice: Ensure the token belongs to this specific doctor
                    hasStandardConsent = consentPrincipal.HasValidConsent(patientId, "MedicalHistory", doctorId);
                }
            }

            // 3. The "Break Glass" Emergency Fallback
            bool hasEmergencyAccess = false;
            if (!hasStandardConsent)
            {
                hasEmergencyAccess = await emergencyAccessService.HasActiveEmergencyAccessAsync(doctorId, patientId);
            }

            // 4. The Gateway Check
            if (!hasStandardConsent && !hasEmergencyAccess)
            {
                return NewResult(new Response<List<TimelineEventDto>>("You do not have a valid, active consent token or emergency override to view this patient's records.")
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                });
            }

            // 5. Fetch the aggregated timeline
            var timeline = await medicalHistoryService.GetPatientTimelineAsync(patientId);

            // 6. Return consistent Response<T> Wrapper
            var resultData = timeline ?? new List<TimelineEventDto>();

            return NewResult(new Response<List<TimelineEventDto>>(resultData)
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "Medical timeline retrieved successfully."
            });
        }
        [HttpGet("my-timeline")]
        [Authorize(Roles = nameof(EnRoles.Patient))]
        public async Task<IActionResult> GetMyTimeline()
        {
            var patientId = await User.GetPatientIdAsync(patientService);

            // 5. Fetch the aggregated timeline
            var timeline = await medicalHistoryService.GetPatientTimelineAsync(patientId);

            // 6. Return consistent Response<T> Wrapper
            var resultData = timeline ?? new List<TimelineEventDto>();

            return NewResult(new Response<List<TimelineEventDto>>(resultData)
            {
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "Medical timeline retrieved successfully."
            });
        }

    }
}
