using Helix.Core.Bases;
using MediatR;

namespace Helix.Core.Features.Observations.Commands.Models
{
    public class DeleteObservationCommand : IRequest<Response<bool>>
    {
        public Guid Id { get; set; }
        public DeleteObservationCommand(Guid id) => Id = id;
    }
}
