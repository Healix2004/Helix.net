using Helix.Core.Bases;
using MediatR;

namespace Helix.Core.Features.Auth.Commands.Models
{
    public class ResendConfirmationEmailCommand : IRequest<Response<string>>
    {
        public string Email { get; set; }

        public ResendConfirmationEmailCommand(string email)
        {
            Email = email;
        }
    }
}

