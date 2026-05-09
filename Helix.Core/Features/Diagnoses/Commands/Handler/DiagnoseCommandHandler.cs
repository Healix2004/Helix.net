using Helix.Core.Bases;
using Helix.Core.Features.Diagnoses.Commands.Models;
using Helix.Service.DTOs.DiagnoseDTOs;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.Diagnoses.Commands.Handler
{
    /// <summary>
    /// Handles <see cref="CreateDiagnoseCommand"/> and creates a new diagnosis record.
    /// </summary>
    public class CreateDiagnoseCommandHandler : IRequestHandler<CreateDiagnoseCommand, Response<DiagnoseDto>>
    {
        private readonly IDiagnoseService _diagnoseService;
        private readonly ResponseHandler _responseHandler;

        public CreateDiagnoseCommandHandler(IDiagnoseService diagnoseService, ResponseHandler responseHandler)
        {
            _diagnoseService = diagnoseService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<DiagnoseDto>> Handle(CreateDiagnoseCommand request, CancellationToken cancellationToken)
        {
            var result = await _diagnoseService.CreateDiagnoseAsync(request.Dto);
            return _responseHandler.Created(result);
        }
    }

    /// <summary>
    /// Handles <see cref="UpdateDiagnoseCommand"/> and updates an existing diagnosis record.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if the record does not exist.
    /// </summary>
    public class UpdateDiagnoseCommandHandler : IRequestHandler<UpdateDiagnoseCommand, Response<DiagnoseDto>>
    {
        private readonly IDiagnoseService _diagnoseService;
        private readonly ResponseHandler _responseHandler;

        public UpdateDiagnoseCommandHandler(IDiagnoseService diagnoseService, ResponseHandler responseHandler)
        {
            _diagnoseService = diagnoseService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<DiagnoseDto>> Handle(UpdateDiagnoseCommand request, CancellationToken cancellationToken)
        {
            var result = await _diagnoseService.UpdateDiagnoseAsync(Guid.Parse(request.Id.ToString()), request.Dto);
            if (result == null)
                return _responseHandler.NotFound<DiagnoseDto>("Diagnose not found.");
            return _responseHandler.Success(result);
        }
    }

    /// <summary>
    /// Handles <see cref="DeleteDiagnoseCommand"/> and removes a diagnosis record.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if the record does not exist.
    /// </summary>
    public class DeleteDiagnoseCommandHandler : IRequestHandler<DeleteDiagnoseCommand, Response<bool>>
    {
        private readonly IDiagnoseService _diagnoseService;
        private readonly ResponseHandler _responseHandler;

        public DeleteDiagnoseCommandHandler(IDiagnoseService diagnoseService, ResponseHandler responseHandler)
        {
            _diagnoseService = diagnoseService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<bool>> Handle(DeleteDiagnoseCommand request, CancellationToken cancellationToken)
        {
            var result = await _diagnoseService.DeleteDiagnoseAsync(Guid.Parse(request.Id.ToString()));
            if (!result)
                return _responseHandler.NotFound<bool>("Diagnose not found.");
            return _responseHandler.Deleted<bool>();
        }
    }
}
