using Helix.Api.Base;
using Helix.Core.Bases;
using Helix.Data.Enums;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helix.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Authorize(Roles = nameof(EnRoles.Doctor))]

    public class LoincController : AppControllerBase
    {
        private readonly ILoincTerminologyService _loincService;

        public LoincController(ILoincTerminologyService loincService)
        {
            _loincService = loincService ?? throw new ArgumentNullException(nameof(loincService));
        }

        [HttpGet("lookup/{code}")]
        [ProducesResponseType(typeof(Response<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LookupLoincCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest(new Response<string>
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Succeeded = false,
                    Message = "LOINC code is required."
                });
            }

            try
            {
                var concept = await _loincService.LookupLoincCodeAsync(code);

                if (concept == null)
                {
                    return NotFound(new Response<string>
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        Succeeded = false,
                        Message = $"LOINC code '{code}' not found."
                    });
                }

                var response = new Response<object>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Succeeded = true,
                    Message = "LOINC code retrieved successfully.",
                    Data = new
                    {
                        Code = concept.Coding?.FirstOrDefault()?.Code,
                        Display = concept.Coding?.FirstOrDefault()?.Display,
                        System = concept.Coding?.FirstOrDefault()?.System,
                        Text = concept.Text
                    }
                };

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new Response<string>
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Succeeded = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(Response<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchLoincCodes([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(new Response<string>
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Succeeded = false,
                    Message = "Search term is required."
                });
            }

            try
            {
                var codes = await _loincService.SearchLoincCodesAsync(searchTerm);
                var codeList = codes?.ToList() ?? new List<Hl7.Fhir.Model.CodeableConcept>();

                var response = new Response<object>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Succeeded = true,
                    Message = $"Found {codeList.Count} LOINC codes matching '{searchTerm}'.",
                    Data = codeList.Select(c => new
                    {
                        Code = c.Coding?.FirstOrDefault()?.Code,
                        Display = c.Coding?.FirstOrDefault()?.Display,
                        System = c.Coding?.FirstOrDefault()?.System,
                        Text = c.Text
                    }).ToList()
                };

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new Response<string>
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Succeeded = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("observation")]
        [ProducesResponseType(typeof(Response<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        public IActionResult GetObservationTemplate([FromQuery] string code, [FromQuery] string display)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest(new Response<string>
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Succeeded = false,
                    Message = "LOINC code is required."
                });
            }

            try
            {
                var observation = _loincService.CreateFhirObservation(code, display);

                var response = new Response<object>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Succeeded = true,
                    Message = "FHIR Observation template created successfully.",
                    Data = new
                    {
                        ResourceType = "Observation",
                        Status = observation.Status?.ToString(),
                        Category = observation.Category?.Select(c => new
                        {
                            Coding = c.Coding?.Select(co => new
                            {
                                System = co.System,
                                Code = co.Code,
                                Display = co.Display
                            })
                        }),
                        Code = new
                        {
                            Coding = observation.Code?.Coding?.Select(c => new
                            {
                                System = c.System,
                                Code = c.Code,
                                Display = c.Display
                            }),
                            Text = observation.Code?.Text
                        },
                        Effective = observation.Effective,
                        Issued = observation.Issued
                    }
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new Response<string>
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Succeeded = false,
                    Message = ex.Message
                });
            }
        }
    }
}
