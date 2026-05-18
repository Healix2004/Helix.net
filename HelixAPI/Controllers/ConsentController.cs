using Helix.Api.Base;
using Helix.Core.Bases;
using Helix.Core.Features.Consents.Commands.Models;
using Helix.Core.Features.Consents.Queries.Models;
using Helix.Data.Entities;
using Helix.Data.Enums;
using Helix.Service.DTOs.ConsentDTOs;
using Helix.Service.Interfaces;
using Helix.Service.Services.PatientService;
using Hl7.Fhir.Utility;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace Helix.API.Controllers
{
    /// <summary>
    /// Manages patient consent records in the Helix healthcare system.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ConsentController(IMediator mediator,ITokenProvider jwtService, IMemoryCache cache,UserManager<AppUser> userManager,IPatientService patientService) : AppControllerBase
    {
        // ==========================================
        // 1. PATIENT WORKFLOW (Generates the QR)
        // ==========================================

        [HttpPost("generate-qr")]
        [Authorize(Roles = nameof(EnRoles.Patient))]
        public async Task<IActionResult> GenerateConsentQr([FromBody] ConsentRequestDto request)
        {
            // 1. Get the current logged-in user and their Patient ID
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userIdString);
            var patient = await patientService.GetPatientByUserIdAsync(userIdString);

            // 2. Pass it all to your newly updated method!
            string realJwt = await jwtService.GenerateConsentToken(user, patient.Id, request);
            
            // 3. Generate a short, 8-character token for the QR code
            string shortQrToken = Guid.NewGuid().ToString("N").Substring(0, 8);

            // 4. Save it in Memory Cache for exactly 5 minutes!
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            cache.Set(shortQrToken, realJwt, cacheOptions);

            // 5. Return it using your standard HELIX Response wrapper!
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
        public IActionResult RedeemQr([FromBody] string scannedQrToken)
        {
            // 1. Check if the token exists in the cache
            if (cache.TryGetValue(scannedQrToken, out string realJwt))
            {
                // 2. BURN THE TOKEN! A QR code can only be used once.
                cache.Remove(scannedQrToken);

                // 3. Give the doctor the real 2-Hour JWT
                var successResponse = new Response<string>()
                {
                    Data=realJwt,
                    Succeeded = true,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Message = "Token redeemed successfully."
                };
                return NewResult(successResponse);
            }

            // If it's not in the cache, it expired or is fake
            var failResponse = new Response<string>(null)
            {
                Succeeded = false,
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                Message = "This QR code is invalid or has expired."
            };
            return NewResult(failResponse);
        }

        [Authorize(Roles = nameof(EnRoles.Admin))]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ConsentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetConsentListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ConsentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetConsentByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ConsentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateConsentDto dto)
        {
            var command = new CreateConsentCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [Authorize(Roles = nameof(EnRoles.Admin))]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ConsentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateConsentDto dto)
        {
            var command = new UpdateConsentCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [Authorize(Roles = nameof(EnRoles.Admin))]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteConsentCommand(id);
            var response = await mediator.Send(command);
            return NewResult(response);
        }
    }
}
