using Helix.Core.Bases;
using Helix.Core.Features.Facilities.Commands.Models;
using Helix.Service.DTOs.FacilitieDTOs;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.Facilities.Commands.Handler
{
    /// <summary>
    /// Handles <see cref="CreateFacilityCommand"/> and creates a new facility record.
    /// </summary>
    public class CreateFacilityCommandHandler : IRequestHandler<CreateFacilityCommand, Response<FacilitieDto>>
    {
        private readonly IFacilitieService _facilitieService;
        private readonly ResponseHandler _responseHandler;

        public CreateFacilityCommandHandler(IFacilitieService facilitieService, ResponseHandler responseHandler)
        {
            _facilitieService = facilitieService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<FacilitieDto>> Handle(CreateFacilityCommand request, CancellationToken cancellationToken)
        {
            var result = await _facilitieService.CreateFacilitieAsync(request.Dto);
            return _responseHandler.Created(result);
        }
    }

    /// <summary>
    /// Handles <see cref="UpdateFacilityCommand"/> and updates an existing facility record.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if the record does not exist.
    /// </summary>
    public class UpdateFacilityCommandHandler : IRequestHandler<UpdateFacilityCommand, Response<FacilitieDto>>
    {
        private readonly IFacilitieService _facilitieService;
        private readonly ResponseHandler _responseHandler;

        public UpdateFacilityCommandHandler(IFacilitieService facilitieService, ResponseHandler responseHandler)
        {
            _facilitieService = facilitieService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<FacilitieDto>> Handle(UpdateFacilityCommand request, CancellationToken cancellationToken)
        {
            var result = await _facilitieService.UpdateFacilitieAsync(Guid.Parse(request.Id.ToString()), request.Dto);
            if (result == null)
                return _responseHandler.NotFound<FacilitieDto>("Facility not found.");
            return _responseHandler.Success(result);
        }
    }

    /// <summary>
    /// Handles <see cref="DeleteFacilityCommand"/> and removes a facility record.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if the record does not exist.
    /// </summary>
    public class DeleteFacilityCommandHandler : IRequestHandler<DeleteFacilityCommand, Response<bool>>
    {
        private readonly IFacilitieService _facilitieService;
        private readonly ResponseHandler _responseHandler;

        public DeleteFacilityCommandHandler(IFacilitieService facilitieService, ResponseHandler responseHandler)
        {
            _facilitieService = facilitieService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<bool>> Handle(DeleteFacilityCommand request, CancellationToken cancellationToken)
        {
            var result = await _facilitieService.DeleteFacilitieAsync(Guid.Parse(request.Id.ToString()));
            if (!result)
                return _responseHandler.NotFound<bool>("Facility not found.");
            return _responseHandler.Deleted<bool>();
        }
    }
}
