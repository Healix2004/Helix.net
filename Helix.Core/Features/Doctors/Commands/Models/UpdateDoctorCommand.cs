using Helix.Core.Bases;
using Helix.Service.DTOs.DoctorDTOs;
using MediatR;

namespace Helix.Core.Features.Doctors.Commands.Models
{
    public class UpdateDoctorCommand : IRequest<Response<DoctorDto>>
    {
        public Guid Id { get; set; }
        public UpdateDoctorDto Dto { get; set; }
        public UpdateDoctorCommand(Guid id, UpdateDoctorDto dto) { Id = id; Dto = dto; }
    }
}
