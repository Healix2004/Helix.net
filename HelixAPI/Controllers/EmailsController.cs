using Helix.Api.Base;
using Helix.Core.Features.Emails.Commands.Models;
using Helix.Data.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace Helix.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailsController : AppControllerBase
    {
        [HttpPost("send")]
        [Authorize(Roles = nameof(EnRoles.Admin))]

        public async Task<IActionResult> SendEmail([FromBody] SendEmailCommand request)
        {
            var response = await mediator.Send(request);
            return NewResult(response);
        }
    }
}
