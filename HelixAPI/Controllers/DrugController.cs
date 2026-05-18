using Helix.Api.Base;
using Helix.Core.Bases; // ADDED: For manual Response<T> wrappers
using Helix.Core.Features.Drugs.Commands.Models;
using Helix.Core.Features.Drugs.Queries.Models;
using Helix.Data.Enums;
using Helix.Service.DTOs.DrugDTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helix.API.Controllers
{

    [Route("api/drugs")] 
    [ApiController]
    public class DrugController(IMediator mediator) : AppControllerBase
    {
        // ==========================================
        // PUBLIC/SHARED WORKFLOW (Directory Lookup)
        // ==========================================

        [HttpGet("all")] // FIX 2: Standardized RESTful path
        [Authorize] // FIX 3: Anyone authenticated in HELIX can read the drug dictionary
        [ProducesResponseType(typeof(IEnumerable<DrugDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetDrugListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        [HttpGet("{id}")]
        [Authorize] // Anyone authenticated in HELIX can read specific drug details
        [ProducesResponseType(typeof(DrugDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetDrugByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }

        // ==========================================
        // ADMIN WORKFLOW (Local Cache Management)
        // ==========================================

        [HttpPost]
        [Authorize(Roles = nameof(EnRoles.Admin))] // FIX 4: Locked write actions to Admins
        [ProducesResponseType(typeof(DrugDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateDrugDto dto)
        {
            var command = new CreateDrugCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = nameof(EnRoles.Admin))]
        [ProducesResponseType(typeof(DrugDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDrugDto dto)
        {
            // FIX 5: Prevent ID Spoofing in the JSON body
            // (Assuming UpdateDrugDto has an Id property. Remove if it doesn't!)
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

            var command = new UpdateDrugCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(EnRoles.Admin))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteDrugCommand(id);
            var response = await mediator.Send(command);
            return NewResult(response);
        }
    }
}