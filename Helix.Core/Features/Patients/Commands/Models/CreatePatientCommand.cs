using Helix.Core.Bases;
using Helix.Service.DTOs.PatientDTOs;
using MediatR;

namespace Helix.Core.Features.Patients.Commands.Models
{
    public class CreatePatientCommand : IRequest<Response<PatientDto>>
    {
        public CreatePatientDto Dto { get; set; }
        public CreatePatientCommand(CreatePatientDto dto) => Dto = dto;
    }
}
