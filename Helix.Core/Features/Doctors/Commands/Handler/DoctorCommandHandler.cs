using Helix.Core.Bases;
using Helix.Core.Features.Doctors.Commands.Models;
using Helix.Service.DTOs.DoctorDTOs;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.Doctors.Commands.Handler
{
    public class CreateDoctorCommandHandler : IRequestHandler<CreateDoctorCommand, Response<DoctorDto>>
    {
        private readonly IDoctorService _doctorService;
        private readonly ResponseHandler _responseHandler;

        public CreateDoctorCommandHandler(IDoctorService doctorService, ResponseHandler responseHandler)
        {
            _doctorService = doctorService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<DoctorDto>> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
        {
            var result = await _doctorService.CreateDoctorAsync(request.Dto);
            return _responseHandler.Created(result);
        }
    }
    public class UpdateDoctorCommandHandler : IRequestHandler<UpdateDoctorCommand, Response<DoctorDto>>
    {
        private readonly IDoctorService _doctorService;
        private readonly ResponseHandler _responseHandler;

        public UpdateDoctorCommandHandler(IDoctorService doctorService, ResponseHandler responseHandler)
        {
            _doctorService = doctorService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<DoctorDto>> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
        {
            var result = await _doctorService.UpdateDoctorAsync(Guid.Parse(request.Id.ToString()), request.Dto);
            if (result == null)
                return _responseHandler.NotFound<DoctorDto>("Doctor not found.");
            return _responseHandler.Success(result);
        }
    }
    public class DeleteDoctorCommandHandler : IRequestHandler<DeleteDoctorCommand, Response<bool>>
    {
        private readonly IDoctorService _doctorService;
        private readonly ResponseHandler _responseHandler;

        public DeleteDoctorCommandHandler(IDoctorService doctorService, ResponseHandler responseHandler)
        {
            _doctorService = doctorService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<bool>> Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
        {
            var result = await _doctorService.DeleteDoctorAsync(Guid.Parse(request.Id.ToString()));
            if (!result)
                return _responseHandler.NotFound<bool>("Doctor not found.");
            return _responseHandler.Deleted<bool>();
        }
    }
}
