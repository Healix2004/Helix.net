using Helix.Core.Bases;
using Helix.Service.DTOs.DiagnoseDTOs;
using MediatR;

namespace Helix.Core.Features.Diagnoses.Commands.Models
{
    public class UpdateDiagnoseCommand : IRequest<Response<DiagnoseDto>>
    {
        public Guid Id { get; set; }
        public UpdateDiagnoseDto Dto { get; set; }
        public UpdateDiagnoseCommand(Guid id, UpdateDiagnoseDto dto) { Id = id; Dto = dto; }
    }
}
