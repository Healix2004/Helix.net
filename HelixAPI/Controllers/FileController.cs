using Helix.Api.Base;
using Helix.Core.Bases;
using Helix.Core.Features.File.Commands.Models;
using Helix.Core.Features.File.Quieres.Models;
using Helix.Service.DTOs.FileDto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Helix.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : AppControllerBase
    {

        /// <summary>
        /// Upload a single file
        /// </summary>
        /// <param name="file">File to upload</param>
        /// <returns>File path if upload successful</returns>
        [HttpPost("upload")]
        [DisableRequestSizeLimit]
        [ProducesResponseType(typeof(Response<FileUploadResult>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Response<FileUploadResult>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Upload([FromForm] FileUploadDto file)
        {
            var command = new UploadFileCommand(file);
            var result = await mediator.Send(command);

            return StatusCode((int)(result.StatusCode), result);
        }

        /// <summary>
        /// Upload multiple files
        /// </summary>
        /// <param name="files">Files to upload</param>
        /// <returns>File paths if upload successful</returns>
        [HttpPost("upload-multiple")]
        [DisableRequestSizeLimit]
        [ProducesResponseType(typeof(Response<MultipleFileUploadResult>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Response<MultipleFileUploadResult>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadMultiple([FromForm] MultipleFileUploadDto files)
        {
            var command = new UploadMultipleFilesCommand(files);
            var result = await mediator.Send(command);

            return StatusCode((int)(result.StatusCode), result);
        }

        /// <summary>
        /// Download a file by file path
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <returns>File stream</returns>
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
                return StatusCode((int)(result.StatusCode), result);
            }

            return File(result.Data.FileStream, result.Data.ContentType, result.Data.FileName);
        }

        /// <summary>
        /// Delete a file by file path
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <returns>Success status</returns>
        [HttpDelete("{*filePath}")]
        [Authorize]
        [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string filePath)
        {
            var command = new DeleteFileCommand(filePath);
            var result = await mediator.Send(command);

            return StatusCode((int)(result.StatusCode), result);
        }

        /// <summary>
        /// Check if a file exists
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <returns>File existence status</returns>
        [HttpGet("exists/{*filePath}")]
        [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Exists(string filePath)
        {
            var query = new FileExistsQuery(filePath);
            var result = await mediator.Send(query);

            return StatusCode((int)(result.StatusCode), result);
        }
    }
}