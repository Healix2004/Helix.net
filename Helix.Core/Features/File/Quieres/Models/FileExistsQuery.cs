using Helix.Core.Bases;
using MediatR;

namespace Helix.Core.Features.File.Quieres.Models
{
    public class FileExistsQuery : IRequest<Response<bool>>
    {
        public string FilePath { get; set; }

        public FileExistsQuery(string filePath)
        {
            FilePath = filePath;
        }
    }
}

