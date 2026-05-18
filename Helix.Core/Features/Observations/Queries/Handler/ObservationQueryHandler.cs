using Helix.Core.Bases;
using Helix.Core.Features.Observations.Queries.Models;
using Helix.Service.DTOs.ObservationDTOs;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.Observations.Queries.Handler
{
    /// <summary>
    /// Handles <see cref="GetObservationListQuery"/> and returns all clinical observation records.
    /// </summary>
    public class GetObservationListQueryHandler : IRequestHandler<GetObservationListQuery, Response<IEnumerable<ObservationDto>>>
    {
        private readonly IObservationService _observationService;
        private readonly ResponseHandler _responseHandler;

        public GetObservationListQueryHandler(IObservationService observationService, ResponseHandler responseHandler)
        {
            _observationService = observationService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<IEnumerable<ObservationDto>>> Handle(GetObservationListQuery request, CancellationToken cancellationToken)
        {
            var result = await _observationService.GetAllObservationsAsync();
            return _responseHandler.Success(result);
        }
    }
    public class GetObservationListForPatientQueryHandler(IObservationService observationService, ResponseHandler responseHandler) : IRequestHandler<GetObservationListForPatientQuery, Response<IEnumerable<ObservationDto>>>
    {
        /// <inheritdoc />
        public async Task<Response<IEnumerable<ObservationDto>>> Handle(GetObservationListForPatientQuery request, CancellationToken cancellationToken)
        {
            var result = await observationService.GetPatientObservationsAsync(request.Id);
            return responseHandler.Success(result);
        }
    }

    /// <summary>
    /// Handles <see cref="GetObservationByIdQuery"/> and returns a single clinical observation record.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if no record exists with the given ID.
    /// </summary>
    public class GetObservationByIdQueryHandler : IRequestHandler<GetObservationByIdQuery, Response<ObservationDto>>
    {
        private readonly IObservationService _observationService;
        private readonly ResponseHandler _responseHandler;

        public GetObservationByIdQueryHandler(IObservationService observationService, ResponseHandler responseHandler)
        {
            _observationService = observationService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<ObservationDto>> Handle(GetObservationByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _observationService.GetObservationByIdAsync(Guid.Parse(request.Id.ToString()));
            if (result == null)
                return _responseHandler.NotFound<ObservationDto>("Observation not found.");
            return _responseHandler.Success(result);
        }
    }
}
