using Helix.Core.Bases;
using Helix.Service.DTOs.FileDto;
using MediatR;

namespace Helix.Core.Features.File.Quieres.Models
{
    public class DownloadFileQuery : IRequest<Response<FileDownloadResult>>
    {
        public string FilePath { get; set; }

        public DownloadFileQuery(string filePath)
        {
            FilePath = filePath;
        }
    }
}

