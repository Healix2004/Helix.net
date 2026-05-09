using Helix.Core.Bases;
using Helix.Core.Features.Doctors.Commands.Models;
using Helix.Core.Features.Doctors.Queries.Models;
using Helix.Service.DTOs.DoctorDTOs;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.Doctors.Queries.Handler
{
    /// <summary>
    /// Handles the <see cref="GetDoctorListQuery"/> and returns all doctor records.
    /// </summary>
    public class GetDoctorListQueryHandler : IRequestHandler<GetDoctorListQuery, Response<IEnumerable<DoctorDto>>>
    {
        private readonly IDoctorService _doctorService;
        private readonly ResponseHandler _responseHandler;

        public GetDoctorListQueryHandler(IDoctorService doctorService, ResponseHandler responseHandler)
        {
            _doctorService = doctorService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<IEnumerable<DoctorDto>>> Handle(GetDoctorListQuery request, CancellationToken cancellationToken)
        {
            var result = await _doctorService.GetAllDoctorsAsync();
            return _responseHandler.Success(result);
        }
    }

    /// <summary>
    /// Handles the <see cref="GetDoctorByIdQuery"/> and returns a single doctor by ID.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if no doctor exists with the given ID.
    /// </summary>
    public class GetDoctorByIdQueryHandler : IRequestHandler<GetDoctorByIdQuery, Response<DoctorDto>>
    {
        private readonly IDoctorService _doctorService;
        private readonly ResponseHandler _responseHandler;

        public GetDoctorByIdQueryHandler(IDoctorService doctorService, ResponseHandler responseHandler)
        {
            _doctorService = doctorService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<DoctorDto>> Handle(GetDoctorByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _doctorService.GetDoctorByIdAsync(Guid.Parse(request.Id.ToString()));
            if (result == null)
                return _responseHandler.NotFound<DoctorDto>("Doctor not found.");
            return _responseHandler.Success(result);
        }
    }
}
