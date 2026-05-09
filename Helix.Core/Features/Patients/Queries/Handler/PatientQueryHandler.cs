using Helix.Core.Bases;
using Helix.Core.Features.Patients.Commands.Models;
using Helix.Core.Features.Patients.Queries.Models;
using Helix.Service.DTOs.PatientDTOs;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.Patients.Queries.Handler
{
    /// <summary>
    /// Handles the <see cref="GetPatientListQuery"/> and returns all patient records.
    /// </summary>
    public class GetPatientListQueryHandler : IRequestHandler<GetPatientListQuery, Response<IEnumerable<PatientDto>>>
    {
        private readonly IPatientService _patientService;
        private readonly ResponseHandler _responseHandler;

        public GetPatientListQueryHandler(IPatientService patientService, ResponseHandler responseHandler)
        {
            _patientService = patientService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<IEnumerable<PatientDto>>> Handle(GetPatientListQuery request, CancellationToken cancellationToken)
        {
            var result = await _patientService.GetAllPatientsAsync();
            return _responseHandler.Success(result);
        }
    }

    /// <summary>
    /// Handles the <see cref="GetPatientByIdQuery"/> and returns a single patient by ID.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if no patient exists with the given ID.
    /// </summary>
    public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, Response<PatientDto>>
    {
        private readonly IPatientService _patientService;
        private readonly ResponseHandler _responseHandler;

        public GetPatientByIdQueryHandler(IPatientService patientService, ResponseHandler responseHandler)
        {
            _patientService = patientService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<PatientDto>> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _patientService.GetPatientByIdAsync(Guid.Parse(request.Id.ToString()));
            if (result == null)
                return _responseHandler.NotFound<PatientDto>("Patient not found.");
            return _responseHandler.Success(result);
        }
    }
}
