using Helix.Core.Bases;
using Helix.Core.DTOs.Terminology;
using Helix.Core.Features.RxNavTerminology.Quieres.Models;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.RxNavTerminology.Quieres.Handler
{
    public class SearchRxNavTerminologyQueryHandler : IRequestHandler<SearchRxNavTerminologyQuery, Response<List<CodingDto>>>
    {
        private readonly ITerminologyService _terminologyService;
        private readonly ResponseHandler _responseHandler;

        public SearchRxNavTerminologyQueryHandler(ITerminologyService terminologyService, ResponseHandler responseHandler)
        {
            _terminologyService = terminologyService;
            _responseHandler = responseHandler;
        }

        public async Task<Response<List<CodingDto>>> Handle(SearchRxNavTerminologyQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return _responseHandler.BadRequest<List<CodingDto>>("Parameter 'text' is required.");
            }

            var items = await _terminologyService.LookupCodesAsync(request.Text, request.System);
            var mapped = items.Select(c => new CodingDto
            {
                System = c.System,
                Code = c.Code,
                Display = c.Display
            }).ToList();

            return _responseHandler.Success(mapped);
        }
    }

    public class ValidateRxNavCodeQueryHandler : IRequestHandler<ValidateRxNavCodeQuery, Response<bool>>
    {
        private readonly ITerminologyService _terminologyService;
        private readonly ResponseHandler _responseHandler;

        public ValidateRxNavCodeQueryHandler(ITerminologyService terminologyService, ResponseHandler responseHandler)
        {
            _terminologyService = terminologyService;
            _responseHandler = responseHandler;
        }

        public async Task<Response<bool>> Handle(ValidateRxNavCodeQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return _responseHandler.BadRequest<bool>("Parameter 'code' is required.");
            }

            var isValid = await _terminologyService.ValidateCodeAsync(request.System, request.Code);
            return _responseHandler.Success(isValid);
        }
    }

    public class GetRxNavDisplayQueryHandler : IRequestHandler<GetRxNavDisplayQuery, Response<string>>
    {
        private readonly ITerminologyService _terminologyService;
        private readonly ResponseHandler _responseHandler;

        public GetRxNavDisplayQueryHandler(ITerminologyService terminologyService, ResponseHandler responseHandler)
        {
            _terminologyService = terminologyService;
            _responseHandler = responseHandler;
        }

        public async Task<Response<string>> Handle(GetRxNavDisplayQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return _responseHandler.BadRequest<string>("Parameter 'code' is required.");
            }

            var display = await _terminologyService.GetDisplayForCodeAsync(request.System, request.Code);
            if (string.IsNullOrWhiteSpace(display))
            {
                return _responseHandler.NotFound<string>("Display not found.");
            }

            return _responseHandler.Success(display);
        }
    }
}
