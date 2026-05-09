using Helix.Core.Bases;
using Helix.Service.DTOs.DiagnoseDTOs;
using MediatR;

namespace Helix.Core.Features.Diagnoses.Commands.Models
{
    public class CreateDiagnoseCommand : IRequest<Response<DiagnoseDto>>
    {
        public CreateDiagnoseDto Dto { get; set; }
        public CreateDiagnoseCommand(CreateDiagnoseDto dto) => Dto = dto;
    }

    public class UpdateDiagnoseCommand : IRequest<Response<DiagnoseDto>>
    {
        public int Id { get; set; }
        public UpdateDiagnoseDto Dto { get; set; }
        public UpdateDiagnoseCommand(int id, UpdateDiagnoseDto dto) { Id = id; Dto = dto; }
    }

    public class DeleteDiagnoseCommand : IRequest<Response<bool>>
    {
        public int Id { get; set; }
        public DeleteDiagnoseCommand(int id) => Id = id;
    }
}
