namespace Helix.Service.DTOs.FileDto
{
    public class MultipleFileUploadResult
    {
        public bool Success { get; set; }
        public List<string> FilePaths { get; set; } = new();
        public string ErrorMessage { get; set; }
    }
}