using Helix.Api.Base;
using Helix.Core.Bases; // ADDED: To use your standard Response<T> wrapper for manual errors
using Helix.Core.Features.TerminologyCodeLookups.Commands.Models;
using Helix.Core.Features.TerminologyCodeLookups.Queries.Models;
using Helix.Data.Enums;
using Helix.Service.DTOs.TerminologyCodeLookupDTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    [Route("api/terminology-code-lookups")] // FIX 1: Explicit RESTful routing
    [ApiController]
    public class TerminologyCodeLookupController(IMediator mediator) : AppControllerBase
    {
        // ==========================================
        // PUBLIC/SHARED WORKFLOW (Directory Lookup)
        // ==========================================

        [HttpGet("all")] // FIX 2: RESTful path
        [Authorize(Roles = nameof(EnRoles.Admin))] 
        [ProducesResponseType(typeof(IEnumerable<TerminologyCodeLookupDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetTerminologyCodeLookupListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }
        [HttpGet("{id}")]
        [Authorize(Roles = nameof(EnRoles.Admin))]
        [ProducesResponseType(typeof(TerminologyCodeLookupDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetTerminologyCodeLookupByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        // ==========================================
        // ADMIN WORKFLOW (System Management)
        // ==========================================
        [HttpPost]
        [Authorize(Roles = nameof(EnRoles.Admin))]
        [ProducesResponseType(typeof(TerminologyCodeLookupDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateTerminologyCodeLookupDto dto)
        {
            var command = new CreateTerminologyCodeLookupCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = nameof(EnRoles.Admin))]
        [ProducesResponseType(typeof(TerminologyCodeLookupDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTerminologyCodeLookupDto dto)
        {
            // FIX 5: Prevent ID Spoofing in the JSON body
            // (Assuming your UpdateTerminologyCodeLookupDto has an Id property. If not, you can remove this check)
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

            var command = new UpdateTerminologyCodeLookupCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(EnRoles.Admin))] // Only Admins can delete codes
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteTerminologyCodeLookupCommand(id);
            var response = await mediator.Send(command);
            return NewResult(response);
        }
    }
}