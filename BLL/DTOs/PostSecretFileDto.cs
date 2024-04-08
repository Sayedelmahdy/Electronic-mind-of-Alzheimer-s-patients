using BLL.ValidationAttributes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class PostSecretFileDto
    {
        public string FileName { get; set; }
        public string File_Description { get; set; }
        [MaxFileSize(50 * 1024 * 1024, ErrorMessage = "File size cannot exceed 50 MB.")]
        public IFormFile File { get; set; } // IFormFile
    }
}
