using Helix.Core.Bases;
using Helix.Core.Features.Emails.Commands.Models;
using Helix.Service.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Helix.Core.Features.Emails.Commands.Handler
{
    public class EmailsCommandHandler(IEmailService emailService) : ResponseHandler, IRequestHandler<SendEmailCommand, Response<string>>
    {
        async Task<Response<string>> IRequestHandler<SendEmailCommand, Response<string>>.Handle([FromQuery] SendEmailCommand request, CancellationToken cancellationToken)
        {
            var response = await emailService.SendEmail(request.Email, request.Message, null);
            if (response == "Success")
            {
                return Success(response);
            }
            else
            {
                return BadRequest<string>("Failed to send email");
            }
        }
    }
}
