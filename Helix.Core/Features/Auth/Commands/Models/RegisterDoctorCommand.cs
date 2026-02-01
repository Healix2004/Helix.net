using Helix.Core.Bases;
using Helix.Service.DTOs.AuthDTOs;
using MediatR;

namespace Helix.Core.Features.Auth.Commands.Models
{
    public class RegisterDoctorCommand : IRequest<Response<AuthDto>>
    {
        public RegisterDoctorDto RegisterDoctorDto {  get; set; }
        public RegisterDoctorCommand(RegisterDoctorDto registerDoctorDto)
        {
            this.RegisterDoctorDto = registerDoctorDto;
        }
    }
}
