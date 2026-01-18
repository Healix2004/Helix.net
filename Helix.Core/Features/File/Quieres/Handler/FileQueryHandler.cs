using Helix.Core.Bases;
using Helix.Core.Features.File.Quieres.Models;
using Helix.Service.DTOs.FileDto;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.File.Quieres.Handler
{
    public class DownloadFileQueryHandler : IRequestHandler<DownloadFileQuery, Response<FileDownloadResult>>
    {
        private readonly IFileService _fileService;
        private readonly ResponseHandler _responseHandler;

        public DownloadFileQueryHandler(IFileService fileService, ResponseHandler responseHandler)
        {
            _fileService = fileService;
            _responseHandler = responseHandler;
        }

        public async Task<Response<FileDownloadResult>> Handle(DownloadFileQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.FilePath))
                {
                    return _responseHandler.BadRequest<FileDownloadResult>("File path is required");
                }

                var result = await _fileService.DownloadFileAsync(request.FilePath);
                
                if (!result.Success)
                {
                    return _responseHandler.NotFound<FileDownloadResult>(result.ErrorMessage ?? "File not found");
                }

                return _responseHandler.Success(result);
            }
            catch (Exception ex)
            {
                return _responseHandler.BadRequest<FileDownloadResult>($"An error occurred during file download: {ex.Message}");
            }
        }
    }

    public class FileExistsQueryHandler : IRequestHandler<FileExistsQuery, Response<bool>>
    {
        private readonly IFileService _fileService;
        private readonly ResponseHandler _responseHandler;

        public FileExistsQueryHandler(IFileService fileService, ResponseHandler responseHandler)
        {
            _fileService = fileService;
            _responseHandler = responseHandler;
        }

        public async Task<Response<bool>> Handle(FileExistsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.FilePath))
                {
                    return _responseHandler.BadRequest<bool>("File path is required");
                }

                var exists = await _fileService.FileExistsAsync(request.FilePath);
                return _responseHandler.Success(exists);
            }
            catch (Exception ex)
            {
                return _responseHandler.BadRequest<bool>($"An error occurred while checking file existence: {ex.Message}");
            }
        }
    }
}
