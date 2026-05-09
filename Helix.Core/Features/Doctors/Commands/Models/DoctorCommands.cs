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

    public class UpdateDoctorCommand : IRequest<Response<DoctorDto>>
    {
        public int Id { get; set; }
        public UpdateDoctorDto Dto { get; set; }
        public UpdateDoctorCommand(int id, UpdateDoctorDto dto) { Id = id; Dto = dto; }
    }

    public class DeleteDoctorCommand : IRequest<Response<bool>>
    {
        public int Id { get; set; }
        public DeleteDoctorCommand(int id) => Id = id;
    }
}
