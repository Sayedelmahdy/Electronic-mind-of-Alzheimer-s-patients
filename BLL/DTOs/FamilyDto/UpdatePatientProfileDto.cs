using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.FamilyDto
{
    public class UpdatePatientProfileDto
    {
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public int Age { get; set; }  
        [Required]
        public DateOnly DiagnosisDate { get; set; }
        [Required]
        public int MaximumDistance { get; set; }
    }
}
