using Helix.Data.Entities;
using Helix.Service.DTOs.FileDto;
using Helix.Service.Services.FileServices;
using Microsoft.AspNetCore.Http;

namespace Helix.Service.Interfaces
{
    public interface IFileService
    {
        Task<FileUploadResult> UploadSingleFileAsync(FileUploadDto file);
        Task<string> UploadFileAsync(IFormFile file);
        Task<MultipleFileUploadResult> UploadMultipleFilesAsync(MultipleFileUploadDto files);
        Task<FileDownloadResult> DownloadFileAsync(string filePath);
        Task<bool> DeleteFileAsync(string filePath);
        Task<bool> FileExistsAsync(string filePath);
        Task<IEnumerable<RadiologyImage>> UploadMultipleFilesAsync(List<IFormFile> files, string PatientName, Guid PatientId);
    }
}