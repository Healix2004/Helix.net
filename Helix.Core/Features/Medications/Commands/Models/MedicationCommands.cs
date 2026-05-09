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

    public class UpdateMedicationCommand : IRequest<Response<MedicationDto>>
    {
        public int Id { get; set; }
        public UpdateMedicationDto Dto { get; set; }
        public UpdateMedicationCommand(int id, UpdateMedicationDto dto) { Id = id; Dto = dto; }
    }

    public class DeleteMedicationCommand : IRequest<Response<bool>>
    {
        public int Id { get; set; }
        public DeleteMedicationCommand(int id) => Id = id;
    }
}
