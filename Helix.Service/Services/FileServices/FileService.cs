using Helix.Service.DTOs.FileDto;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Helix.Service.Services.FileServices
{
    public class FileService(IWebHostEnvironment environment, ILogger<FileService> logger) : IFileService
    {


        public async Task<FileUploadResult> UploadSingleFileAsync(FileUploadDto file)
        {
            try
            {
                if (file?.file == null || file.file.Length == 0)
                {
                    return new FileUploadResult
                    {
                        Success = false,
                        ErrorMessage = "Please upload a valid file."
                    };
                }

                // Validate file
                var validationResult = ValidateFile(file.file);
                if (!validationResult.IsValid)
                {
                    return new FileUploadResult
                    {
                        Success = false,
                        ErrorMessage = validationResult.ErrorMessage
                    };
                }

                var (folderPath, savePath, fullPath, dbPath) = GenerateFilePaths(file.file.FileName);

                // Ensure directory exists
                Directory.CreateDirectory(savePath);

                // Delete if file already exists
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                // Save file
                await using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.file.CopyToAsync(stream);
                }

                logger.LogInformation("File uploaded successfully: {FilePath}", dbPath);

                return new FileUploadResult
                {
                    Success = true,
                    FilePath = dbPath
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error uploading single file: {FileName}", file?.file?.FileName);
                return new FileUploadResult
                {
                    Success = false,
                    ErrorMessage = $"Error uploading file: {ex.Message}"
                };
            }
        }

        public async Task<MultipleFileUploadResult> UploadMultipleFilesAsync(MultipleFileUploadDto files)
        {
            try
            {
                if (files?.files == null || !files.files.Any())
                {
                    return new MultipleFileUploadResult
                    {
                        Success = false,
                        ErrorMessage = "Please upload valid files."
                    };
                }

                var dbPaths = new List<string>();
                var errors = new List<string>();

                foreach (var file in files.files)
                {
                    var validationResult = ValidateFile(file);
                    if (!validationResult.IsValid)
                    {
                        errors.Add($"File {file.FileName}: {validationResult.ErrorMessage}");
                        continue;
                    }

                    try
                    {
                        var (folderPath, savePath, fullPath, dbPath) = GenerateFilePaths(file.FileName);

                        // Ensure directory exists
                        Directory.CreateDirectory(savePath);

                        // Delete if file already exists
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }

                        // Save file
                        await using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        dbPaths.Add(dbPath);
                        logger.LogInformation("File uploaded successfully: {FilePath}", dbPath);
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"File {file.FileName}: {ex.Message}");
                        logger.LogError(ex, "Error uploading file: {FileName}", file.FileName);
                    }
                }

                return new MultipleFileUploadResult
                {
                    Success = errors.Count == 0,
                    FilePaths = dbPaths,
                    ErrorMessage = errors.Any() ? string.Join("; ", errors) : null
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error uploading multiple files");
                return new MultipleFileUploadResult
                {
                    Success = false,
                    ErrorMessage = $"Error uploading files: {ex.Message}"
                };
            }
        }

        public async Task<FileDownloadResult> DownloadFileAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    return new FileDownloadResult
                    {
                        Success = false,
                        ErrorMessage = "File path is required."
                    };
                }

                var fullPath = Path.Combine(environment.ContentRootPath, filePath);

                if (!System.IO.File.Exists(fullPath))
                {
                    return new FileDownloadResult
                    {
                        Success = false,
                        ErrorMessage = "File not found."
                    };
                }

                var memory = new MemoryStream();
                await using (var stream = new FileStream(fullPath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                var contentType = GetContentType(fullPath);
                var fileName = Path.GetFileName(fullPath);

                logger.LogInformation("File downloaded successfully: {FilePath}", filePath);

                return new FileDownloadResult
                {
                    Success = true,
                    FileStream = memory,
                    ContentType = contentType,
                    FileName = fileName
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error downloading file: {FilePath}", filePath);
                return new FileDownloadResult
                {
                    Success = false,
                    ErrorMessage = $"Error downloading file: {ex.Message}"
                };
            }
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    return false;

                var fullPath = Path.Combine(environment.ContentRootPath, filePath);

                if (!System.IO.File.Exists(fullPath))
                    return false;

                System.IO.File.Delete(fullPath);
                logger.LogInformation("File deleted successfully: {FilePath}", filePath);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
                return false;
            }
        }

        public async Task<bool> FileExistsAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    return false;

                var fullPath = Path.Combine(environment.ContentRootPath, filePath);
                return System.IO.File.Exists(fullPath);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error checking file existence: {FilePath}", filePath);
                return false;
            }
        }

        #region Private Methods

        private (string folderPath, string savePath, string fullPath, string dbPath) GenerateFilePaths(string fileName)
        {
            var folderPath = Path.Combine("Uploads", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"));
            var savePath = Path.Combine(environment.ContentRootPath, folderPath);
            var fullPath = Path.Combine(savePath, fileName);
            var dbPath = Path.Combine(folderPath, fileName);

            return (folderPath, savePath, fullPath, dbPath);
        }

        private (bool IsValid, string ErrorMessage) ValidateFile(IFormFile file)
        {
            if (file == null)
                return (false, "File is required.");

            if (file.Length == 0)
                return (false, "File is empty.");

            // Validate file size (10MB limit)
            const long maxFileSize = 10 * 1024 * 1024; // 10MB
            if (file.Length > maxFileSize)
                return (false, $"File size exceeds the maximum limit of {maxFileSize / (1024 * 1024)}MB.");

            // Validate file extension
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".doc", ".docx", ".txt", ".xls", ".xlsx" };
            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            
            if (string.IsNullOrEmpty(extension))
                return (false, "File must have a valid extension.");

            if (!allowedExtensions.Contains(extension))
                return (false, $"File type '{extension}' is not allowed. Allowed types: {string.Join(", ", allowedExtensions)}.");

            // Validate file name
            if (string.IsNullOrWhiteSpace(file.FileName) || file.FileName.Length > 255)
                return (false, "File name is invalid or too long.");

            return (true, null);
        }

        private string GetContentType(string path)
        {
            var extension = Path.GetExtension(path).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".txt" => "text/plain",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream",
            };
        }

        #endregion
    }
}