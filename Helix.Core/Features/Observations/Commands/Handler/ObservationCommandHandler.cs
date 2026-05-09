using Helix.Core.Bases;
using Helix.Core.Features.Observations.Commands.Models;
using Helix.Service.DTOs.ObservationDTOs;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.Observations.Commands.Handler
{
    /// <summary>
    /// Handles <see cref="CreateObservationCommand"/> and creates a new clinical observation record.
    /// </summary>
    public class CreateObservationCommandHandler : IRequestHandler<CreateObservationCommand, Response<ObservationDto>>
    {
        private readonly IObservationService _observationService;
        private readonly ResponseHandler _responseHandler;

        public CreateObservationCommandHandler(IObservationService observationService, ResponseHandler responseHandler)
        {
            _observationService = observationService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<ObservationDto>> Handle(CreateObservationCommand request, CancellationToken cancellationToken)
        {
            var result = await _observationService.CreateObservationAsync(request.Dto);
            return _responseHandler.Created(result);
        }
    }

    /// <summary>
    /// Handles <see cref="UpdateObservationCommand"/> and updates an existing observation record.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if the record does not exist.
    /// </summary>
    public class UpdateObservationCommandHandler : IRequestHandler<UpdateObservationCommand, Response<ObservationDto>>
    {
        private readonly IObservationService _observationService;
        private readonly ResponseHandler _responseHandler;

        public UpdateObservationCommandHandler(IObservationService observationService, ResponseHandler responseHandler)
        {
            _observationService = observationService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<ObservationDto>> Handle(UpdateObservationCommand request, CancellationToken cancellationToken)
        {
            var result = await _observationService.UpdateObservationAsync(Guid.Parse(request.Id.ToString()), request.Dto);
            if (result == null)
                return _responseHandler.NotFound<ObservationDto>("Observation not found.");
            return _responseHandler.Success(result);
        }
    }

    /// <summary>
    /// Handles <see cref="DeleteObservationCommand"/> and removes an observation record.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if the record does not exist.
    /// </summary>
    public class DeleteObservationCommandHandler : IRequestHandler<DeleteObservationCommand, Response<bool>>
    {
        private readonly IObservationService _observationService;
        private readonly ResponseHandler _responseHandler;

        public DeleteObservationCommandHandler(IObservationService observationService, ResponseHandler responseHandler)
        {
            _observationService = observationService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<bool>> Handle(DeleteObservationCommand request, CancellationToken cancellationToken)
        {
            var result = await _observationService.DeleteObservationAsync(Guid.Parse(request.Id.ToString()));
            if (!result)
                return _responseHandler.NotFound<bool>("Observation not found.");
            return _responseHandler.Deleted<bool>();
        }
    }
}
