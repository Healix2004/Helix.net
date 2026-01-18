using Helix.Core.Bases;
using Helix.Service.DTOs.FileDto;
using MediatR;

namespace Helix.Core.Features.File.Commands.Models
{
    public class UploadFileCommand : IRequest<Response<FileUploadResult>>
    {
        public FileUploadDto FileUploadDto { get; set; }

        public UploadFileCommand(FileUploadDto fileUploadDto)
        {
            FileUploadDto = fileUploadDto;
        }
    }
}
