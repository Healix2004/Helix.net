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
}
