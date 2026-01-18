using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helix.Service.DTOs.FileDto
{
    public class FileUploadDto
    {
        public IFormFile file { get; set; }
        public string FileName { get; set; }
        public string Discription { get; set; }
        public string Author { get; set; }
    }
}
