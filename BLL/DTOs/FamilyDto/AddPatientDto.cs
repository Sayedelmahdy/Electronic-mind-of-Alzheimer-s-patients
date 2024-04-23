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
    public class AddPatientDto
    {
        [Required, StringLength(100)]
        public string FullName { get; set; }

        [Required, StringLength(128)]
        public string Email { get; set; }

        [Required, StringLength(256)]
        public string Password { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public double MainLongitude { get; set; }
        [Required]
        public double MainLatitude { get; set; }
        [Required(ErrorMessage = "Please select a file.")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg", ".mp4", ".mkv" }, ErrorMessage = "Valid file extensions are: .jpg, .png, .jpeg, .mp4, .mkv")]
        [MaxFileSize(50 * 1024 * 1024, ErrorMessage = "File size cannot exceed 50 MB.")]

        public IFormFile Avatar { get; set; }
        [Required]
        public string relationality { get; set; }
       
        public string? DescriptionForPatient { get; set; }
        [Required]
        public DateOnly DiagnosisDate { get; set; }
        [Required]
        public int MaximumDistance { get; set; }
    }
}
