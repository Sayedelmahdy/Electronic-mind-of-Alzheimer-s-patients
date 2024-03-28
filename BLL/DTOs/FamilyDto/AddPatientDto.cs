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
        public IFormFile Avatar { get; set; }
        [Required]
       public string relationality { get; set; }
        [Required]
        public DateOnly DiagnosisDate { get; set; }
        [Required]
        public int MaximumDistance { get; set; }
    }
}
