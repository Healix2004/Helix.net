using Helix.Core.Bases;
using Helix.Service.DTOs.FileDto;
using MediatR;

namespace Helix.Core.Features.File.Commands.Models
{
    public class UploadMultipleFilesCommand : IRequest<Response<MultipleFileUploadResult>>
    {
        public MultipleFileUploadDto MultipleFileUploadDto { get; set; }

        public UploadMultipleFilesCommand(MultipleFileUploadDto multipleFileUploadDto)
        {
            MultipleFileUploadDto = multipleFileUploadDto;
        }
    }
}

