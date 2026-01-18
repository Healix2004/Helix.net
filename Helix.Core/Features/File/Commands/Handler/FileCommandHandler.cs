using Helix.Core.Bases;
using Helix.Core.Features.File.Commands.Models;
using Helix.Service.DTOs.FileDto;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.File.Commands.Handler
{
    public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, Response<FileUploadResult>>
    {
        private readonly IFileService _fileService;
        private readonly ResponseHandler _responseHandler;

        public UploadFileCommandHandler(IFileService fileService, ResponseHandler responseHandler)
        {
            _fileService = fileService;
            _responseHandler = responseHandler;
        }

        public async Task<Response<FileUploadResult>> Handle(UploadFileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _fileService.UploadSingleFileAsync(request.FileUploadDto);
                
                if (!result.Success)
                {
                    return _responseHandler.BadRequest<FileUploadResult>(result.ErrorMessage ?? "File upload failed");
                }

                return _responseHandler.Created(result);
            }
            catch (Exception ex)
            {
                return _responseHandler.BadRequest<FileUploadResult>($"An error occurred during file upload: {ex.Message}");
            }
        }
    }

    public class UploadMultipleFilesCommandHandler : IRequestHandler<UploadMultipleFilesCommand, Response<MultipleFileUploadResult>>
    {
        private readonly IFileService _fileService;
        private readonly ResponseHandler _responseHandler;

        public UploadMultipleFilesCommandHandler(IFileService fileService, ResponseHandler responseHandler)
        {
            _fileService = fileService;
            _responseHandler = responseHandler;
        }

        public async Task<Response<MultipleFileUploadResult>> Handle(UploadMultipleFilesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _fileService.UploadMultipleFilesAsync(request.MultipleFileUploadDto);
                
                if (!result.Success)
                {
                    return _responseHandler.BadRequest<MultipleFileUploadResult>(result.ErrorMessage ?? "File upload failed");
                }

                return _responseHandler.Created(result);
            }
            catch (Exception ex)
            {
                return _responseHandler.BadRequest<MultipleFileUploadResult>($"An error occurred during file upload: {ex.Message}");
            }
        }
    }

    public class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand, Response<bool>>
    {
        private readonly IFileService _fileService;
        private readonly ResponseHandler _responseHandler;

        public DeleteFileCommandHandler(IFileService fileService, ResponseHandler responseHandler)
        {
            _fileService = fileService;
            _responseHandler = responseHandler;
        }

        public async Task<Response<bool>> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _fileService.DeleteFileAsync(request.FilePath);
                
                if (!result)
                {
                    return _responseHandler.NotFound<bool>("File not found or could not be deleted");
                }

                return _responseHandler.Deleted<bool>();
            }
            catch (Exception ex)
            {
                return _responseHandler.BadRequest<bool>($"An error occurred during file deletion: {ex.Message}");
            }
        }
    }
}
