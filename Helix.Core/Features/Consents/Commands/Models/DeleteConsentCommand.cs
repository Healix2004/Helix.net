using Helix.Core.Bases;
using MediatR;

namespace Helix.Core.Features.Consents.Commands.Models
{
    public class DeleteConsentCommand : IRequest<Response<bool>>
    {
        public Guid Id { get; set; }
        public DeleteConsentCommand(Guid id) => Id = id;
    }
}
