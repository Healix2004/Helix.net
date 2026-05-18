using Helix.Core.Bases;
using Helix.Core.Features.Diagnoses.Queries.Models;
using Helix.Service.DTOs.DiagnoseDTOs;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.Diagnoses.Queries.Handler
{
    public class GetDiagnoseListQueryHandler : IRequestHandler<GetDiagnoseListQuery, Response<IEnumerable<DiagnoseDto>>>
    {
        private readonly IDiagnoseService _diagnoseService;
        private readonly ResponseHandler _responseHandler;

        public GetDiagnoseListQueryHandler(IDiagnoseService diagnoseService, ResponseHandler responseHandler)
        {
            _diagnoseService = diagnoseService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<IEnumerable<DiagnoseDto>>> Handle(GetDiagnoseListQuery request, CancellationToken cancellationToken)
        {
            var result = await _diagnoseService.GetAllDiagnosesAsync();
            return _responseHandler.Success(result);
        }
    }
    public class GetDiagnoseByIdQueryHandler : IRequestHandler<GetDiagnoseByIdQuery, Response<DiagnoseDto>>
    {
        private readonly IDiagnoseService _diagnoseService;
        private readonly ResponseHandler _responseHandler;

        public GetDiagnoseByIdQueryHandler(IDiagnoseService diagnoseService, ResponseHandler responseHandler)
        {
            _diagnoseService = diagnoseService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<DiagnoseDto>> Handle(GetDiagnoseByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _diagnoseService.GetDiagnoseByIdAsync(request.Id);
            if (result == null)
                return _responseHandler.NotFound<DiagnoseDto>("Diagnose not found.");
            return _responseHandler.Success(result);
        }
    }

    public class GetDiagnoseListForPatientQueryHandler(IDiagnoseService diagnoseService, ResponseHandler responseHandler) : IRequestHandler<GetDiagnoseListForPatientQuery, Response<IEnumerable<DiagnoseDto>>>
    {
        /// <inheritdoc />
        public async Task<Response<IEnumerable<DiagnoseDto>>> Handle(GetDiagnoseListForPatientQuery request, CancellationToken cancellationToken)
        {
            var result = await diagnoseService.GetPatientDiagnosesAsync(request.PatiendId);
            return responseHandler.Success(result);
        }
    }
}
