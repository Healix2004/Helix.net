using Helix.Api.Base;
using Helix.Core.Features.Facilities.Commands.Models;
using Helix.Core.Features.Facilities.Queries.Models;
using Helix.Data.Enums;
using Helix.Service.DTOs.FacilitieDTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    /// <summary>
    /// Manages healthcare facility records in the HELIX healthcare ecosystem.
    /// </summary>
    [Route("api/facilities")] // FIX 1: Explicit RESTful routing
    [ApiController]
    // [Authorize(Roles = nameof(EnRoles.Admin))] <-- REMOVED: This blocked patients from seeing locations!
    public class FacilityController(IMediator mediator) : AppControllerBase
    {
        // ==========================================================
        // PUBLIC/SHARED WORKFLOW (Directory Lookup)
        // ==========================================================

        [HttpGet("all")]
        [Authorize] // FIX 2: Anyone logged into HELIX (Admin, Doctor, Patient) can see the list of facilities
        [ProducesResponseType(typeof(IEnumerable<FacilitieDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetFacilityListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpGet("{id}")]
        [Authorize] // Anyone logged into HELIX can view a specific facility's details
        [ProducesResponseType(typeof(FacilitieDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetFacilityByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        // ==========================================
        // ADMIN WORKFLOW (System Management)
        // ==========================================

        [HttpPost]
        [Authorize(Roles = nameof(EnRoles.Admin))] // FIX 3: Explicitly locked write-actions to Admins
        [ProducesResponseType(typeof(FacilitieDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateFacilitieDto dto)
        {
            var command = new CreateFacilityCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = nameof(EnRoles.Admin))]
        [ProducesResponseType(typeof(FacilitieDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFacilitieDto dto)
        {
            // Security check: Prevent ID spoofing
            // Assuming your UpdateFacilitieDto has an Id property. If it doesn't, you can remove this check!
            if (id != dto.Id)
            {
                return BadRequest("The ID in the URL does not match the ID in the body.");
            }

            var command = new UpdateFacilityCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(EnRoles.Admin))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteFacilityCommand(id);
            var response = await mediator.Send(command);
            return NewResult(response);
        }
    }
}