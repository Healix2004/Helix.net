using Helix.Core.Bases;
using Helix.Service.DTOs.MedicationDTOs;
using MediatR;

namespace Helix.Core.Features.Medications.Commands.Models
{
    public class CreateMedicationCommand : IRequest<Response<MedicationDto>>
    {
        public CreateMedicationDto Dto { get; set; }
        public CreateMedicationCommand(CreateMedicationDto dto) => Dto = dto;
    }
}
