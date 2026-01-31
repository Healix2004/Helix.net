using Helix.Api.Base;
using Helix.Data.Enums;
using Helix.Service.DTOs.DrugDTOs;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(EnRoles.Doctor))]
    public class DrugController(IDrugDataService drugService) : AppControllerBase
    {
        /// <summary>
        /// Check for drug interactions between the provided drugs
        /// </summary>
        [HttpPost("check-interaction")]
        [ProducesResponseType(typeof(InteractionResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckInteraction([FromBody] InteractionRequestDTO request)
        {
            var response = await drugService.CheckDrugInteractionAsync(request);
            if (response == null)
            {
                return BadRequest();
            }
            return Ok(response);
        }

        /// <summary>
        /// Get the list of available drugs
        /// </summary>
        [HttpGet("drugs")]
        [ProducesResponseType(typeof(List<DrugDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDrugs()
        {
            var drugs = await drugService.ImportDrugDataAsync();
            return Ok(drugs);
        }

        /// <summary>
        /// Set the drug interaction server IP address
        /// </summary>
        [HttpPost("set-server-ip")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetServerIP([FromBody] ServerIpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Ip))
            {
                return BadRequest("IP address is required.");
            }
            await drugService.SetServerIP(request.Ip);
            var res = new { res = " IP Address Setted Correctly" };
            return Ok(res);
        }
        [HttpGet("get-server-ip")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetServerIP()
        {
            var ip = await drugService.GetServerIP();
            var res = new { Ip = ip };
            return StatusCode((int)StatusCodes.Status200OK, res);
        }
    }

    public record ServerIpRequest(string Ip);
}
