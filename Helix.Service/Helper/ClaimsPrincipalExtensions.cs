using Helix.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Helix.Service.Helper
{
    public static class ClaimsPrincipalExtensions
    {
        // 1. The Patient Extension
        public static async Task<Guid> GetPatientIdAsync(this ClaimsPrincipal user, IPatientService patientService)
        {
            var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString))
                throw new UnauthorizedAccessException("User ID claim is missing from the token.");

            // FIX: Using await instead of .Result
            var patient = await patientService.GetPatientByUserIdAsync(userIdString);

            if (patient != null)
                return patient.Id;

            throw new UnauthorizedAccessException("Invalid patient ID.");
        }

        // 2. The Doctor Extension
        public static async Task<Guid> GetDoctorIdAsync(this ClaimsPrincipal user, IDoctorService doctorService)
        {
            var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString))
                throw new UnauthorizedAccessException("User ID claim is missing from the token.");

            // FIX: Using await instead of .Result
            var doctor = await doctorService.GetDoctorByUserIdAsync(userIdString);

            if (doctor != null)
                return doctor.Id;

            throw new UnauthorizedAccessException("Invalid doctor ID.");
        }
        // 3. the 
        // Notice the "this ClaimsPrincipal user" parameter. This is the magic!
        public static bool HasValidConsent(this ClaimsPrincipal user, Guid requestedPatientId, string requiredScope)
        {
            var tokenType = user.FindFirst("TokenType")?.Value;
            if (tokenType != "PatientConsent")
                return false;

            var allowedPatientId = user.FindFirst("PatientId")?.Value;
            if (string.IsNullOrEmpty(allowedPatientId) || allowedPatientId != requestedPatientId.ToString())
                return false;

            var grantedScopes = user.FindAll("GrantedScope").Select(c => c.Value).ToList();
            if (!grantedScopes.Contains(requiredScope))
                return false;

            return true;
        }
        public static bool HasValidConsent(this ClaimsPrincipal consentPrincipal, Guid requestedPatientId, string requiredScope, Guid? requestingDoctorId = null)
        {
            // 1. Verify this is actually a consent token, not a standard login token
            var tokenType = consentPrincipal.FindFirst("TokenType")?.Value;
            if (tokenType != "PatientConsent") return false;

            // 2. Verify the token belongs to the requested patient
            var allowedPatientId = consentPrincipal.FindFirst("PatientId")?.Value;
            if (string.IsNullOrEmpty(allowedPatientId) || allowedPatientId != requestedPatientId.ToString())
                return false;

            // 3. Verify the token includes the required scope (e.g., "Labs")
            var grantedScopes = consentPrincipal.FindAll("GrantedScope").Select(c => c.Value).ToList();
            if (!grantedScopes.Contains(requiredScope)) return false;

            // 4. (Optional but Secure) If the token was generated for a specific doctor, enforce it
            var targetDoctorId = consentPrincipal.FindFirst("TargetDoctorId")?.Value;
            if (!string.IsNullOrEmpty(targetDoctorId) && requestingDoctorId.HasValue)
            {
                if (targetDoctorId != requestingDoctorId.Value.ToString()) return false;
            }

            return true;
        }
    }
}