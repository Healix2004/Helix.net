using Helix.Core.Bases;
using Helix.Core.Features.LabTestResults.Commands.Models;
using Helix.Service.DTOs.LabTestResultDTOs;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.LabTestResults.Commands.Handler
{
    /// <summary>
    /// Handles <see cref="CreateLabTestResultCommand"/> and creates a new lab test result record.
    /// </summary>
    public class CreateLabTestResultCommandHandler : IRequestHandler<CreateLabTestResultCommand, Response<LabTestResultDto>>
    {
        private readonly ILabTestResultService _labTestResultService;
        private readonly ResponseHandler _responseHandler;

        public CreateLabTestResultCommandHandler(ILabTestResultService labTestResultService, ResponseHandler responseHandler)
        {
            _labTestResultService = labTestResultService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<LabTestResultDto>> Handle(CreateLabTestResultCommand request, CancellationToken cancellationToken)
        {
            var result = await _labTestResultService.CreateLabTestResultAsync(request.Dto);
            return _responseHandler.Created(result);
        }
    }

    /// <summary>
    /// Handles <see cref="UpdateLabTestResultCommand"/> and updates an existing lab test result record.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if the record does not exist.
    /// </summary>
    public class UpdateLabTestResultCommandHandler : IRequestHandler<UpdateLabTestResultCommand, Response<LabTestResultDto>>
    {
        private readonly ILabTestResultService _labTestResultService;
        private readonly ResponseHandler _responseHandler;

        public UpdateLabTestResultCommandHandler(ILabTestResultService labTestResultService, ResponseHandler responseHandler)
        {
            _labTestResultService = labTestResultService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<LabTestResultDto>> Handle(UpdateLabTestResultCommand request, CancellationToken cancellationToken)
        {
            var result = await _labTestResultService.UpdateLabTestResultAsync(Guid.Parse(request.Id.ToString()), request.Dto);
            if (result == null)
                return _responseHandler.NotFound<LabTestResultDto>("Lab test result not found.");
            return _responseHandler.Success(result);
        }
    }

    /// <summary>
    /// Handles <see cref="DeleteLabTestResultCommand"/> and removes a lab test result record.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if the record does not exist.
    /// </summary>
    public class DeleteLabTestResultCommandHandler : IRequestHandler<DeleteLabTestResultCommand, Response<bool>>
    {
        private readonly ILabTestResultService _labTestResultService;
        private readonly ResponseHandler _responseHandler;

        public DeleteLabTestResultCommandHandler(ILabTestResultService labTestResultService, ResponseHandler responseHandler)
        {
            _labTestResultService = labTestResultService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<bool>> Handle(DeleteLabTestResultCommand request, CancellationToken cancellationToken)
        {
            var result = await _labTestResultService.DeleteLabTestResultAsync(Guid.Parse(request.Id.ToString()));
            if (!result)
                return _responseHandler.NotFound<bool>("Lab test result not found.");
            return _responseHandler.Deleted<bool>();
        }
    }
}
