using Helix.Core.Bases;
using Helix.Service.DTOs.AuthDTOs;
using MediatR;

namespace Helix.Core.Features.Auth.Commands.Models
{
    public class ChangePasswordCommand : IRequest<Response<string>>
    {
        public ChangePasswordDto ChangePasswordDto { get; set; }
        public string UserId { get; set; }

        public ChangePasswordCommand(ChangePasswordDto changePasswordDto, string userId)
        {
            ChangePasswordDto = changePasswordDto;
            UserId = userId;
        }
    }
}

