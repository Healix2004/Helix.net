using Helix.Core.Bases;
using Helix.Service.DTOs.AuthDTOs;
using MediatR;

namespace Helix.Core.Features.Auth.Commands.Models
{
    public class ConfirmEmailCommand : IRequest<Response<string>>
    {
        public ConfirmEmailDto ConfirmEmailDto { get; set; }

        public ConfirmEmailCommand(ConfirmEmailDto confirmEmailDto)
        {
            ConfirmEmailDto = confirmEmailDto;
        }
    }
}

