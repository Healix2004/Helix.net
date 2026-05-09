using Helix.Core.Bases;
using Helix.Service.DTOs.LabTestResultDTOs;
using MediatR;

namespace Helix.Core.Features.LabTestResults.Commands.Models
{
    public class CreateLabTestResultCommand : IRequest<Response<LabTestResultDto>>
    {
        public CreateLabTestResultDto Dto { get; set; }
        public CreateLabTestResultCommand(CreateLabTestResultDto dto) => Dto = dto;
    }

    public class UpdateLabTestResultCommand : IRequest<Response<LabTestResultDto>>
    {
        public int Id { get; set; }
        public UpdateLabTestResultDto Dto { get; set; }
        public UpdateLabTestResultCommand(int id, UpdateLabTestResultDto dto) { Id = id; Dto = dto; }
    }

    public class DeleteLabTestResultCommand : IRequest<Response<bool>>
    {
        public int Id { get; set; }
        public DeleteLabTestResultCommand(int id) => Id = id;
    }
}
