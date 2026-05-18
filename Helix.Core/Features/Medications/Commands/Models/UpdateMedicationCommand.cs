using Helix.Core.Bases;
using Helix.Service.DTOs.MedicationDTOs;
using MediatR;

namespace Helix.Core.Features.Medications.Commands.Models
{
    public class UpdateMedicationCommand : IRequest<Response<MedicationDto>>
    {
        public Guid Id { get; set; }
        public UpdateMedicationDto Dto { get; set; }
        public UpdateMedicationCommand(Guid id, UpdateMedicationDto dto) { Id = id; Dto = dto; }
    }
}
