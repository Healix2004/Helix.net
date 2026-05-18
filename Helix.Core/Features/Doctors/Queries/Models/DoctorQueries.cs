using Helix.Core.Bases;
using Helix.Service.DTOs.DoctorDTOs;
using MediatR;

namespace Helix.Core.Features.Doctors.Queries.Models
{
    public class GetDoctorListQuery : IRequest<Response<IEnumerable<DoctorDto>>> { }

    public class GetDoctorByIdQuery : IRequest<Response<DoctorDto>>
    {
        public Guid Id { get; set; }
        public GetDoctorByIdQuery(Guid id) => Id = id;
    }
}
