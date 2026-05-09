using Helix.Core.Bases;
using MediatR;

namespace Helix.Core.Features.Patients.Commands.Models
{
    public class DeletePatientCommand : IRequest<Response<bool>>
    {
        public int Id { get; set; }
        public DeletePatientCommand(int id) => Id = id;
    }
}
