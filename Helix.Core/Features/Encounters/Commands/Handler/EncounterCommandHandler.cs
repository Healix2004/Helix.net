using Helix.Core.Bases;
using Helix.Core.Features.Encounters.Commands.Models;
using Helix.Service.DTOs.EncounterDTOs;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.Encounters.Commands.Handler
{
    /// <summary>
    /// Handles <see cref="CreateEncounterCommand"/> and creates a new encounter record.
    /// </summary>
    public class CreateEncounterCommandHandler : IRequestHandler<CreateEncounterCommand, Response<EncounterDto>>
    {
        private readonly IEncounterService _encounterService;
        private readonly ResponseHandler _responseHandler;

        public CreateEncounterCommandHandler(IEncounterService encounterService, ResponseHandler responseHandler)
        {
            _encounterService = encounterService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<EncounterDto>> Handle(CreateEncounterCommand request, CancellationToken cancellationToken)
        {
            var result = await _encounterService.CreateEncounterAsync(request.Dto);
            return _responseHandler.Created(result);
        }
    }

    /// <summary>
    /// Handles <see cref="UpdateEncounterCommand"/> and updates an existing encounter record.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if the record does not exist.
    /// </summary>
    public class UpdateEncounterCommandHandler : IRequestHandler<UpdateEncounterCommand, Response<EncounterDto>>
    {
        private readonly IEncounterService _encounterService;
        private readonly ResponseHandler _responseHandler;

        public UpdateEncounterCommandHandler(IEncounterService encounterService, ResponseHandler responseHandler)
        {
            _encounterService = encounterService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<EncounterDto>> Handle(UpdateEncounterCommand request, CancellationToken cancellationToken)
        {
            var result = await _encounterService.UpdateEncounterAsync(Guid.Parse(request.Id.ToString()), request.Dto);
            if (result == null)
                return _responseHandler.NotFound<EncounterDto>("Encounter not found.");
            return _responseHandler.Success(result);
        }
    }

    /// <summary>
    /// Handles <see cref="DeleteEncounterCommand"/> and removes an encounter record.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if the record does not exist.
    /// </summary>
    public class DeleteEncounterCommandHandler : IRequestHandler<DeleteEncounterCommand, Response<bool>>
    {
        private readonly IEncounterService _encounterService;
        private readonly ResponseHandler _responseHandler;

        public DeleteEncounterCommandHandler(IEncounterService encounterService, ResponseHandler responseHandler)
        {
            _encounterService = encounterService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<bool>> Handle(DeleteEncounterCommand request, CancellationToken cancellationToken)
        {
            var result = await _encounterService.DeleteEncounterAsync(Guid.Parse(request.Id.ToString()));
            if (!result)
                return _responseHandler.NotFound<bool>("Encounter not found.");
            return _responseHandler.Deleted<bool>();
        }
    }
}
