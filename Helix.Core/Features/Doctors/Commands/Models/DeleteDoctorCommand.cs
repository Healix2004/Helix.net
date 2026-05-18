using Helix.Core.Bases;
using MediatR;

namespace Helix.Core.Features.Doctors.Commands.Models
{
    public class DeleteDoctorCommand : IRequest<Response<bool>>
    {
        public Guid Id { get; set; }
        public DeleteDoctorCommand(Guid id) => Id = id;
    }
}
