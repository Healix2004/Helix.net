using Helix.Core.Bases;
using MediatR;

namespace Helix.Core.Features.Patients.Commands.Models
{
    public class DeletePatientCommand : IRequest<Response<bool>>
    {
        public Guid Id { get; set; }
        public DeletePatientCommand(Guid id) => Id = id;
    }
}
