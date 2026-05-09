using Helix.Core.Bases;
using Helix.Core.Features.Allergies.Commands.Models;
using Helix.Service.DTOs.AllergyDTOs;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.Allergies.Commands.Handler
{
    /// <summary>
    /// Handles <see cref="CreateAllergyCommand"/> and creates a new allergy record.
    /// </summary>
    public class CreateAllergyCommandHandler : IRequestHandler<CreateAllergyCommand, Response<AllergyDto>>
    {
        private readonly IAllergyService _allergyService;
        private readonly ResponseHandler _responseHandler;

        public CreateAllergyCommandHandler(IAllergyService allergyService, ResponseHandler responseHandler)
        {
            _allergyService = allergyService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<AllergyDto>> Handle(CreateAllergyCommand request, CancellationToken cancellationToken)
        {
            var result = await _allergyService.CreateAllergyAsync(request.Dto);
            return _responseHandler.Created(result);
        }
    }

    /// <summary>
    /// Handles <see cref="UpdateAllergyCommand"/> and updates an existing allergy record.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if the record does not exist.
    /// </summary>
    public class UpdateAllergyCommandHandler : IRequestHandler<UpdateAllergyCommand, Response<AllergyDto>>
    {
        private readonly IAllergyService _allergyService;
        private readonly ResponseHandler _responseHandler;

        public UpdateAllergyCommandHandler(IAllergyService allergyService, ResponseHandler responseHandler)
        {
            _allergyService = allergyService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<AllergyDto>> Handle(UpdateAllergyCommand request, CancellationToken cancellationToken)
        {
            var result = await _allergyService.UpdateAllergyAsync(Guid.Parse(request.Id.ToString()), request.Dto);
            if (result == null)
                return _responseHandler.NotFound<AllergyDto>("Allergy not found.");
            return _responseHandler.Success(result);
        }
    }

    /// <summary>
    /// Handles <see cref="DeleteAllergyCommand"/> and removes an allergy record.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if the record does not exist.
    /// </summary>
    public class DeleteAllergyCommandHandler : IRequestHandler<DeleteAllergyCommand, Response<bool>>
    {
        private readonly IAllergyService _allergyService;
        private readonly ResponseHandler _responseHandler;

        public DeleteAllergyCommandHandler(IAllergyService allergyService, ResponseHandler responseHandler)
        {
            _allergyService = allergyService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<bool>> Handle(DeleteAllergyCommand request, CancellationToken cancellationToken)
        {
            var result = await _allergyService.DeleteAllergyAsync(Guid.Parse(request.Id.ToString()));
            if (!result)
                return _responseHandler.NotFound<bool>("Allergy not found.");
            return _responseHandler.Deleted<bool>();
        }
    }
}
