using Helix.Core.Bases;
using MediatR;

namespace Helix.Core.Features.Diagnoses.Commands.Models
{
    public class DeleteDiagnoseCommand : IRequest<Response<bool>>
    {
        public Guid Id { get; set; }
        public DeleteDiagnoseCommand(Guid id) => Id = id;
    }
}
