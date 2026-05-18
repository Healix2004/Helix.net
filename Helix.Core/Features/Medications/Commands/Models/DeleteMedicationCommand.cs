using Helix.Core.Bases;
using MediatR;

namespace Helix.Core.Features.Medications.Commands.Models
{
    public class DeleteMedicationCommand : IRequest<Response<bool>>
    {
        public Guid Id { get; set; }
        public DeleteMedicationCommand(Guid id) => Id = id;
    }
}
