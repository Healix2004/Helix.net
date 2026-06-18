using Helix.Core.Bases;
using Helix.Service.DTOs.AuthDTOs;
using Helix.Service.DTOs.DoctorDTOs;
using MediatR;

namespace Helix.Core.Features.Auth.Commands.Models
{
    public class RegisterDoctorCommand : IRequest<Response<AuthDto>>
    {
        public DoctorRegistrationPayloadDto RegisterDoctorDto {  get; set; }
        public RegisterDoctorCommand(DoctorRegistrationPayloadDto registerDoctorDto)
        {
            this.RegisterDoctorDto = registerDoctorDto;
        }
    }
}
