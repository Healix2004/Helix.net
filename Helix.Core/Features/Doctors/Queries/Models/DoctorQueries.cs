using Helix.Core.Bases;
using Helix.Service.DTOs.DoctorDTOs;
using MediatR;

namespace Helix.Core.Features.Doctors.Queries.Models
{
    public class GetDoctorListQuery : IRequest<Response<IEnumerable<DoctorDto>>> { }

    public class GetDoctorByIdQuery : IRequest<Response<DoctorDto>>
    {
        public int Id { get; set; }
        public GetDoctorByIdQuery(int id) => Id = id;
    }
}
