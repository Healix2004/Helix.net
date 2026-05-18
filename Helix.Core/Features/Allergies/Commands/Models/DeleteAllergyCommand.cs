using Helix.Core.Bases;
using MediatR;

namespace Helix.Core.Features.Allergies.Commands.Models
{
    public class DeleteAllergyCommand : IRequest<Response<bool>>
    {
        public Guid Id { get; set; }
        public DeleteAllergyCommand(Guid id) => Id = id;
    }
}
