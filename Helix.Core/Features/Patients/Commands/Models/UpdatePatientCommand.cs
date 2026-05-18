using Helix.Core.Bases;
using Helix.Service.DTOs.PatientDTOs;
using MediatR;

namespace Helix.Core.Features.Patients.Commands.Models
{
    public class UpdatePatientCommand : IRequest<Response<PatientDto>>
    {
        public Guid Id { get; set; }
        public UpdatePatientDto Dto { get; set; }
        public UpdatePatientCommand(Guid id, UpdatePatientDto dto) { Id = id; Dto = dto; }
    }
}
