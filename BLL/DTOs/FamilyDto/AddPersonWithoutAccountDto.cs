using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.FamilyDto
{
    public class AddPersonWithoutAccountDto
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public List<IFormFile> TraningImage { get; set; }
        [Required]
        public double MainLongitude { get; set; } 
        [Required]
        public double MainLatitude { get; set; }
        [Required]
        public string Relationility { get; set; }

        public string? DescriptionForPatient { get; set; }
    }
}
