using Helix.Data.Entities;
using Helix.Service.DTOs.FileDto;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IEnumerable<RadiologyImage>> UploadMultipleFilesAsync(List<IFormFile> files , string PatientName, Guid PatientId)
        {
            try
            {
                if (files == null || !files.Any())
                {

                }

                var result = new List<RadiologyImage>();
                var errors = new List<string>();

                foreach (var file in files)
                {
                    var validationResult = ValidateFile(file);
                    if (!validationResult.IsValid)
                    {
                        errors.Add($"File {file.FileName}: {validationResult.ErrorMessage}");
                        continue;
                    }

                    try
                    {
                        var (folderPath, savePath, fullPath, dbPath) = GenerateFilePaths(file.FileName,PatientName,PatientId);

                        // Ensure directory exists
                        Directory.CreateDirectory(savePath);

                        // Save file
                        await using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        result.Add(new RadiologyImage()
                        {
                            FilePath = dbPath,
                            FileName = Path.GetFileName(fullPath),
                            FileSizeInKB = file.Length / 1024
                        });
                        logger.LogInformation("File uploaded successfully: {FilePath}", dbPath);
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"File {file.FileName}: {ex.Message}");
                        logger.LogError(ex, "Error uploading file: {FileName}", file.FileName);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error uploading multiple files");
                return new List<RadiologyImage>();
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

                // FIXED PATH RESOLUTION
                var relativePath = filePath.TrimStart('/', '\\');
                var fullPath = Path.Combine(environment.WebRootPath, relativePath);

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

                // FIXED PATH RESOLUTION
                var relativePath = filePath.TrimStart('/', '\\');
                var fullPath = Path.Combine(environment.WebRootPath, relativePath);

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

                // FIXED PATH RESOLUTION
                var relativePath = filePath.TrimStart('/', '\\');
                var fullPath = Path.Combine(environment.WebRootPath, relativePath);

                return System.IO.File.Exists(fullPath);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error checking file existence: {FilePath}", filePath);
                return false;
            }
        }

        #region Private Methods

        private (string folderPath, string savePath, string fullPath, string dbPath) GenerateFilePaths(string originalFileName)
        {
            // 1. Define the relative folder structure (e.g., "Uploads\2026\05")
            var folderPath = Path.Combine("Uploads", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"), DateTime.Now.ToString("dd"));

            // 2. Combine it with WebRootPath to target the wwwroot folder
            var savePath = Path.Combine(environment.WebRootPath, folderPath);

            // 3. Generate a unique filename to prevent overwriting other patients' files
            var extension = Path.GetExtension(originalFileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";

            // 4. The exact physical path on the server where the file will be written
            var fullPath = Path.Combine(savePath, uniqueFileName);

            // 5. Create a web-safe URL path for the database (e.g., "/Uploads/2026/05/uuid.jpg")
            var dbPath = $"/{folderPath.Replace("\\", "/")}/{uniqueFileName}";

            return (folderPath, savePath, fullPath, dbPath);
        }

        private string SanitizeFolderName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Unknown_Patient";

            // 1. Remove characters that are illegal in Windows/Linux folder names
            var invalidChars = Path.GetInvalidFileNameChars();
            var cleanName = new string(name.Where(ch => !invalidChars.Contains(ch)).ToArray());

            // 2. Replace spaces with underscores so your web URLs are clean (no "%20")
            return cleanName.Replace(" ", "_");
        }
        private (string folderPath, string savePath, string fullPath, string dbPath) GenerateFilePaths(string originalFileName, string patientName, Guid patientId)
        {
            string safePatientName = SanitizeFolderName(patientName);

            // 2. THE FIX: Combine the name and the unique ID!
            string uniquePatientFolder = $"{safePatientName}_{patientId}";

            // 3. Use the unique folder here
            var folderPath = Path.Combine("Uploads", uniquePatientFolder, DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"), DateTime.Now.ToString("dd"));

            var savePath = Path.Combine(environment.WebRootPath, folderPath);
            var extension = Path.GetExtension(originalFileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(savePath, uniqueFileName);
            var dbPath = $"/{folderPath.Replace("\\", "/")}/{uniqueFileName}";

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