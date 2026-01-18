using Helix.Core.Features.Emails.Commands.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Helix.Api.Base;
namespace Helix.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailsController : AppControllerBase
    {
        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailCommand request)
        {
            var response = await mediator.Send(request);
            return NewResult(response);
        }
    }
}
