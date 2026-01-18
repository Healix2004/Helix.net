using Helix.Core.Bases;
using Helix.Service.DTOs.AuthDTOs;
using MediatR;

namespace Helix.Core.Features.Auth.Commands.Models
{
    public class ForgetPasswordCommand : IRequest<Response<string>>
    {
        public ForgetPasswordDto ForgetPasswordDto { get; set; }

        public ForgetPasswordCommand(ForgetPasswordDto forgetPasswordDto)
        {
            ForgetPasswordDto = forgetPasswordDto;
        }
    }
}

