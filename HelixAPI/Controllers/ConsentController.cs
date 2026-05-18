using Helix.Api.Base;
using Helix.Core.Bases;
using Helix.Core.Features.Consents.Commands.Models;
using Helix.Core.Features.Consents.Queries.Models;
using Helix.Data.Entities;
using Helix.Data.Enums;
using Helix.Service.DTOs.ConsentDTOs;
using Helix.Service.Helper;
using Helix.Service.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Helix.API.Controllers
{
    /// <summary>
    /// Manages patient consent records and temporary access tokens in the HELIX ecosystem.
    /// </summary>
    [Route("api/consents")] // FIX 1: Explicit RESTful routing
    [ApiController]
    public class ConsentController(
        IMediator mediator,
        ITokenProvider jwtService,
        IMemoryCache cache,
        UserManager<AppUser> userManager,
        IPatientService patientService,
        IDoctorService doctorService,
        IConsentService consentService) : AppControllerBase
    {
        // ==========================================
        // 1. PATIENT WORKFLOW (Generates the QR)
        // ==========================================

        [HttpPost("generate-qr")]
        [Authorize(Roles = nameof(EnRoles.Patient))]
        public async Task<IActionResult> GenerateConsentQr([FromBody] ConsentRequestDto request)
        {
            // 1. Safely extract the Patient ID using your new Extension Method!
            Guid patientId = await User.GetPatientIdAsync(patientService);

            // 2. We still need the AppUser object for the token generation claims
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userIdString);

            // 3. Generate the massive 2-Hour JWT
            string realJwt = await jwtService.GenerateConsentToken(user, patientId, request);

            // 4. Generate a short, 8-character token for the QR code
            string shortQrToken = Guid.NewGuid().ToString("N").Substring(0, 8);

            // 5. Save it in Memory Cache for exactly 5 minutes!
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            cache.Set(shortQrToken, realJwt, cacheOptions);

            var response = new Response<string>()
            {
                Data = shortQrToken,
                Succeeded = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "QR Token generated. Valid for 5 minutes."
            };

            return NewResult(response);
        }

        // ==========================================
        // 2. DOCTOR WORKFLOW (Redeems the QR)
        // ==========================================

        [HttpPost("redeem-qr")]
        [Authorize(Roles = nameof(EnRoles.Doctor))]
        public async Task<IActionResult> RedeemQr([FromBody] string scannedQrToken)
        {
            if (cache.TryGetValue(scannedQrToken, out string realJwt))
            {
                // BURN THE TOKEN! A QR code can only be used once.
                cache.Remove(scannedQrToken);

                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(realJwt);

                // 1. Look for patientId (added a fallback just in case the case is different)
                var patientIdClaim = token.Claims.FirstOrDefault(c =>
                    c.Type == "patientId" ||
                    c.Type.Equals("patientid", StringComparison.OrdinalIgnoreCase))?.Value;

                // 2. We no longer check for expClaim here, because token.ValidTo handles it!
                if (string.IsNullOrEmpty(patientIdClaim))
                {
                    var invalidResponse = new Response<string>(null)
                    {
                        Succeeded = false,
                        StatusCode = System.Net.HttpStatusCode.BadRequest,
                        Message = "The token does not contain the required patientId claim."
                    };
                    return NewResult(invalidResponse);
                }

                var successResponse = new Response<string>()
                {
                    Succeeded = true,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Message = "Token redeemed successfully.",
                    Data= realJwt
                };

                var consent = new CreateConsentDto()
                {
                    ConsentGrantedAt = DateTime.UtcNow,
                    DoctorId = await User.GetDoctorIdAsync(doctorService),
                    PatientId = Guid.Parse(patientIdClaim),
                    ConsentExpiresAt = token.ValidTo
                };

                await consentService.CreateConsentAsync(consent);
                return NewResult(successResponse);
            }

            var failResponse = new Response<string>(null)
            {
                Succeeded = false,
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                Message = "This QR code is invalid or has expired."
            };
            return NewResult(failResponse);
        }

        // ==========================================
        // 3. ADMIN WORKFLOW (Database Management)
        // ==========================================

        [HttpGet("all")] // RESTful path
        [Authorize(Roles = nameof(EnRoles.Admin))]
        [ProducesResponseType(typeof(IEnumerable<ConsentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetConsentListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = nameof(EnRoles.Admin))] // FIX 2: Locked this down!
        [ProducesResponseType(typeof(ConsentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetConsentByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpPost]
        [Authorize(Roles = nameof(EnRoles.Admin))] // FIX 3: Prevented anonymous record creation!
        [ProducesResponseType(typeof(ConsentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateConsentDto dto)
        {
            var command = new CreateConsentCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = nameof(EnRoles.Admin))]
        [ProducesResponseType(typeof(ConsentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateConsentDto dto)
        {
            // FIX 4: Security check to prevent ID spoofing
            if (id != dto.Id)
            {
                return BadRequest("The ID in the URL does not match the ID in the body.");
            }

            var command = new UpdateConsentCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(EnRoles.Admin))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteConsentCommand(id);
            var response = await mediator.Send(command);
            return NewResult(response);
        }
    }
}