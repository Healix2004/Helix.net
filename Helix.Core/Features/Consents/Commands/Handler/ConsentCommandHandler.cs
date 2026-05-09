using Helix.Core.Bases;
using Helix.Core.Features.Consents.Commands.Models;
using Helix.Service.DTOs.ConsentDTOs;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.Consents.Commands.Handler
{
    /// <summary>
    /// Handles <see cref="CreateConsentCommand"/> and creates a new consent record.
    /// </summary>
    public class CreateConsentCommandHandler : IRequestHandler<CreateConsentCommand, Response<ConsentDto>>
    {
        private readonly IConsentService _consentService;
        private readonly ResponseHandler _responseHandler;

        public CreateConsentCommandHandler(IConsentService consentService, ResponseHandler responseHandler)
        {
            _consentService = consentService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<ConsentDto>> Handle(CreateConsentCommand request, CancellationToken cancellationToken)
        {
            var result = await _consentService.CreateConsentAsync(request.Dto);
            return _responseHandler.Created(result);
        }
    }

    /// <summary>
    /// Handles <see cref="UpdateConsentCommand"/> and updates an existing consent record.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if the record does not exist.
    /// </summary>
    public class UpdateConsentCommandHandler : IRequestHandler<UpdateConsentCommand, Response<ConsentDto>>
    {
        private readonly IConsentService _consentService;
        private readonly ResponseHandler _responseHandler;

        public UpdateConsentCommandHandler(IConsentService consentService, ResponseHandler responseHandler)
        {
            _consentService = consentService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<ConsentDto>> Handle(UpdateConsentCommand request, CancellationToken cancellationToken)
        {
            var result = await _consentService.UpdateConsentAsync(Guid.Parse(request.Id.ToString()), request.Dto);
            if (result == null)
                return _responseHandler.NotFound<ConsentDto>("Consent not found.");
            return _responseHandler.Success(result);
        }
    }

    /// <summary>
    /// Handles <see cref="DeleteConsentCommand"/> and removes a consent record.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if the record does not exist.
    /// </summary>
    public class DeleteConsentCommandHandler : IRequestHandler<DeleteConsentCommand, Response<bool>>
    {
        private readonly IConsentService _consentService;
        private readonly ResponseHandler _responseHandler;

        public DeleteConsentCommandHandler(IConsentService consentService, ResponseHandler responseHandler)
        {
            _consentService = consentService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<bool>> Handle(DeleteConsentCommand request, CancellationToken cancellationToken)
        {
            var result = await _consentService.DeleteConsentAsync(Guid.Parse(request.Id.ToString()));
            if (!result)
                return _responseHandler.NotFound<bool>("Consent not found.");
            return _responseHandler.Deleted<bool>();
        }
    }
}
