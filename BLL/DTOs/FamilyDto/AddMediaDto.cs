using BLL.ValidationAttributes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.FamilyDto
{
    public class AddMediaDto
    {
        
        [Required(ErrorMessage = "Please select a file.")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg", ".mp4", ".mkv" }, ErrorMessage = "Valid file extensions are: .jpg, .png, .jpeg, .mp4, .mkv")]
        [MaxFileSize(50 * 1024 * 1024, ErrorMessage = "File size cannot exceed 50 MB.")]
        public IFormFile MediaFile {  get; set; }
        [Required]
       public string Caption { get; set; }  
        

    }
}
