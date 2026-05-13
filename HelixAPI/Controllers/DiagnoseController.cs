using Helix.Api.Base;
using Helix.Core.Features.Diagnoses.Commands.Models;
using Helix.Core.Features.Diagnoses.Queries.Models;
using Helix.Service.DTOs.DiagnoseDTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    /// <summary>
    /// Manages patient diagnosis records in the Helix healthcare system.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DiagnoseController(IMediator mediator) : AppControllerBase
    {
 
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DiagnoseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetDiagnoseListQuery();
            var response = await mediator.Send(query);
            return NewResult(response);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DiagnoseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetDiagnoseByIdQuery(id);
            var response = await mediator.Send(query);
            return NewResult(response);
        }


        [HttpPost]
        [ProducesResponseType(typeof(DiagnoseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateDiagnoseDto dto)
        {
            var command = new CreateDiagnoseCommand(dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

 
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(DiagnoseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDiagnoseDto dto)
        {
            var command = new UpdateDiagnoseCommand(id, dto);
            var response = await mediator.Send(command);
            return NewResult(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteDiagnoseCommand(id);
            var response = await mediator.Send(command);
            return NewResult(response);
        }
    }
}
