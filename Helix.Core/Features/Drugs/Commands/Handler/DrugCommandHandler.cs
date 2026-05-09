using Helix.Core.Bases;
using Helix.Core.Features.Drugs.Commands.Models;
using Helix.Service.DTOs.DrugDTOs;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.Drugs.Commands.Handler
{
    /// <summary>
    /// Stub handler for <see cref="CreateDrugCommand"/>.
    /// Drug creation is not supported by the current external read-only drug data service (<see cref="IDrugDataService"/>).
    /// Implement when a full CRUD drug service is available.
    /// </summary>
    public class CreateDrugCommandHandler : IRequestHandler<CreateDrugCommand, Response<DrugDTO>>
    {
        private readonly ResponseHandler _responseHandler;

        public CreateDrugCommandHandler(ResponseHandler responseHandler)
        {
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public Task<Response<DrugDTO>> Handle(CreateDrugCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_responseHandler.BadRequest<DrugDTO>("Drug creation is managed by the external drug data service."));
        }
    }

    /// <summary>
    /// Stub handler for <see cref="UpdateDrugCommand"/>.
    /// Drug updates are not supported by the current external read-only drug data service (<see cref="IDrugDataService"/>).
    /// Implement when a full CRUD drug service is available.
    /// </summary>
    public class UpdateDrugCommandHandler : IRequestHandler<UpdateDrugCommand, Response<DrugDTO>>
    {
        private readonly ResponseHandler _responseHandler;

        public UpdateDrugCommandHandler(ResponseHandler responseHandler)
        {
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public Task<Response<DrugDTO>> Handle(UpdateDrugCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_responseHandler.BadRequest<DrugDTO>("Drug update is managed by the external drug data service."));
        }
    }

    /// <summary>
    /// Stub handler for <see cref="DeleteDrugCommand"/>.
    /// Drug deletion is not supported by the current external read-only drug data service (<see cref="IDrugDataService"/>).
    /// Implement when a full CRUD drug service is available.
    /// </summary>
    public class DeleteDrugCommandHandler : IRequestHandler<DeleteDrugCommand, Response<bool>>
    {
        private readonly ResponseHandler _responseHandler;

        public DeleteDrugCommandHandler(ResponseHandler responseHandler)
        {
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public Task<Response<bool>> Handle(DeleteDrugCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_responseHandler.BadRequest<bool>("Drug deletion is managed by the external drug data service."));
        }
    }
}
