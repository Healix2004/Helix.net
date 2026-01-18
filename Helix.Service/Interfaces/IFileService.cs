using Helix.Service.DTOs.FileDto;

namespace Helix.Service.Interfaces
{
    public interface IFileService
    {
        Task<FileUploadResult> UploadSingleFileAsync(FileUploadDto file);
        Task<MultipleFileUploadResult> UploadMultipleFilesAsync(MultipleFileUploadDto files);
        Task<FileDownloadResult> DownloadFileAsync(string filePath);
        Task<bool> DeleteFileAsync(string filePath);
        Task<bool> FileExistsAsync(string filePath);
    }
}