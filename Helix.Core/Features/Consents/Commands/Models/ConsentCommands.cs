using Helix.Core.Bases;
using Helix.Service.DTOs.ConsentDTOs;
using MediatR;

namespace Helix.Core.Features.Consents.Commands.Models
{
    public class CreateConsentCommand : IRequest<Response<ConsentDto>>
    {
        public CreateConsentDto Dto { get; set; }
        public CreateConsentCommand(CreateConsentDto dto) => Dto = dto;
    }

    public class UpdateConsentCommand : IRequest<Response<ConsentDto>>
    {
        public int Id { get; set; }
        public UpdateConsentDto Dto { get; set; }
        public UpdateConsentCommand(int id, UpdateConsentDto dto) { Id = id; Dto = dto; }
    }

    public class DeleteConsentCommand : IRequest<Response<bool>>
    {
        public int Id { get; set; }
        public DeleteConsentCommand(int id) => Id = id;
    }
}
