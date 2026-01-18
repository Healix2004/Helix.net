using Helix.Core.Bases;
using Helix.Service.DTOs.AuthDTOs;
using MediatR;

namespace Helix.Core.Features.Auth.Commands.Models
{
    public class ResetPasswordCommand : IRequest<Response<string>>
    {
        public ResetPasswordDto ResetPasswordDto { get; set; }

        public ResetPasswordCommand(ResetPasswordDto resetPasswordDto)
        {
            ResetPasswordDto = resetPasswordDto;
        }
    }
}

