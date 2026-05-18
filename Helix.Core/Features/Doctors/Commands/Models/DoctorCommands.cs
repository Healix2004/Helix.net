using Helix.Core.Bases;
using Helix.Service.DTOs.DoctorDTOs;
using MediatR;

namespace Helix.Core.Features.Doctors.Commands.Models
{
    public class CreateDoctorCommand : IRequest<Response<DoctorDto>>
    {
        public CreateDoctorDto Dto { get; set; }
        public CreateDoctorCommand(CreateDoctorDto dto) => Dto = dto;
    }
}
