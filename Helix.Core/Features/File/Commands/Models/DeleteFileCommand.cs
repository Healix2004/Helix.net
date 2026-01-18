using Helix.Core.Bases;
using MediatR;

namespace Helix.Core.Features.File.Commands.Models
{
    public class DeleteFileCommand : IRequest<Response<bool>>
    {
        public string FilePath { get; set; }

        public DeleteFileCommand(string filePath)
        {
            FilePath = filePath;
        }
    }
}

