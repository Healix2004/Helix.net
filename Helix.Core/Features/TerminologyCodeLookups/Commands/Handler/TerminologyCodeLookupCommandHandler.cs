using Helix.Core.Bases;
using Helix.Core.Features.TerminologyCodeLookups.Commands.Models;
using Helix.Service.DTOs.TerminologyCodeLookupDTOs;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.TerminologyCodeLookups.Commands.Handler
{
    /// <summary>
    /// Handles <see cref="CreateTerminologyCodeLookupCommand"/> and creates a new terminology code lookup entry.
    /// </summary>
    public class CreateTerminologyCodeLookupCommandHandler : IRequestHandler<CreateTerminologyCodeLookupCommand, Response<TerminologyCodeLookupDto>>
    {
        private readonly ITerminologyCodeLookupService _terminologyService;
        private readonly ResponseHandler _responseHandler;

        public CreateTerminologyCodeLookupCommandHandler(ITerminologyCodeLookupService terminologyService, ResponseHandler responseHandler)
        {
            _terminologyService = terminologyService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<TerminologyCodeLookupDto>> Handle(CreateTerminologyCodeLookupCommand request, CancellationToken cancellationToken)
        {
            var result = await _terminologyService.CreateTerminologyCodeLookupAsync(request.Dto);
            return _responseHandler.Created(result);
        }
    }

    /// <summary>
    /// Handles <see cref="UpdateTerminologyCodeLookupCommand"/> and updates an existing terminology code lookup entry.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if the record does not exist.
    /// </summary>
    public class UpdateTerminologyCodeLookupCommandHandler : IRequestHandler<UpdateTerminologyCodeLookupCommand, Response<TerminologyCodeLookupDto>>
    {
        private readonly ITerminologyCodeLookupService _terminologyService;
        private readonly ResponseHandler _responseHandler;

        public UpdateTerminologyCodeLookupCommandHandler(ITerminologyCodeLookupService terminologyService, ResponseHandler responseHandler)
        {
            _terminologyService = terminologyService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<TerminologyCodeLookupDto>> Handle(UpdateTerminologyCodeLookupCommand request, CancellationToken cancellationToken)
        {
            var result = await _terminologyService.UpdateTerminologyCodeLookupAsync(Guid.Parse(request.Id.ToString()), request.Dto);
            if (result == null)
                return _responseHandler.NotFound<TerminologyCodeLookupDto>("Terminology code lookup not found.");
            return _responseHandler.Success(result);
        }
    }

    /// <summary>
    /// Handles <see cref="DeleteTerminologyCodeLookupCommand"/> and removes a terminology code lookup entry.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if the record does not exist.
    /// </summary>
    public class DeleteTerminologyCodeLookupCommandHandler : IRequestHandler<DeleteTerminologyCodeLookupCommand, Response<bool>>
    {
        private readonly ITerminologyCodeLookupService _terminologyService;
        private readonly ResponseHandler _responseHandler;

        public DeleteTerminologyCodeLookupCommandHandler(ITerminologyCodeLookupService terminologyService, ResponseHandler responseHandler)
        {
            _terminologyService = terminologyService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<bool>> Handle(DeleteTerminologyCodeLookupCommand request, CancellationToken cancellationToken)
        {
            var result = await _terminologyService.DeleteTerminologyCodeLookupAsync(Guid.Parse(request.Id.ToString()));
            if (!result)
                return _responseHandler.NotFound<bool>("Terminology code lookup not found.");
            return _responseHandler.Deleted<bool>();
        }
    }
}
