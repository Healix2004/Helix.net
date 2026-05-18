using Helix.Api.Base;
using Helix.Core.Bases;
using Helix.Core.Features.File.Commands.Models;
using Helix.Core.Features.File.Quieres.Models; // Kept your original namespace mapping
using Helix.Data.Enums;
using Helix.Service.DTOs.FileDto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Helix.API.Controllers
{
    [Route("api/files")] // FIX 1: Explicit RESTful routing
    [ApiController]
    [Authorize] // FIX 2: Lock down the ENTIRE controller! Nobody unauthenticated should touch files.
    public class FileController(IMediator mediator) : AppControllerBase // FIX 3: Added the Primary Constructor for IMediator!
    {
        [HttpPost("upload")]
        [DisableRequestSizeLimit] // Note: In production, it is safer to set a hard limit (e.g., 50MB) rather than disable it completely.
        [ProducesResponseType(typeof(Response<FileUploadResult>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Response<FileUploadResult>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Upload([FromForm] FileUploadDto file)
        {
            var command = new UploadFileCommand(file);
            var result = await mediator.Send(command);

            // FIX 4: Replaced StatusCode() with your clean NewResult() wrapper
            return NewResult(result);
        }

        [HttpPost("upload-multiple")]
        [DisableRequestSizeLimit]
        [ProducesResponseType(typeof(Response<MultipleFileUploadResult>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Response<MultipleFileUploadResult>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadMultiple([FromForm] MultipleFileUploadDto files)
        {
            var command = new UploadMultipleFilesCommand(files);
            var result = await mediator.Send(command);

            return NewResult(result);
        }

        [HttpGet("download/{*filePath}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<FileDownloadResult>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Response<FileDownloadResult>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Download(string filePath)
        {
            var query = new DownloadFileQuery(filePath);
            var result = await mediator.Send(query);

            if (!result.Succeeded || result.Data == null)
            {
                // If it fails, return your standard JSON error wrapper
                return NewResult(result);
            }

            // If it succeeds, return the actual FileStream so the browser downloads it!
            return File(result.Data.FileStream, result.Data.ContentType, result.Data.FileName);
        }

        [HttpDelete("{*filePath}")]
        [Authorize(Roles =nameof(EnRoles.Admin))]
        [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string filePath)
        {
            var command = new DeleteFileCommand(filePath);
            var result = await mediator.Send(command);

            return NewResult(result);
        }

        [HttpGet("exists/{*filePath}")]
        [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Exists(string filePath)
        {
            var query = new FileExistsQuery(filePath);
            var result = await mediator.Send(query);

            return NewResult(result);
        }
    }
}