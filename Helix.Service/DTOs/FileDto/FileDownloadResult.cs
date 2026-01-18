namespace Helix.Service.DTOs.FileDto
{
    public class FileDownloadResult
    {
        public bool Success { get; set; }
        public MemoryStream FileStream { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public string ErrorMessage { get; set; }
    }
}