using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.PatientDto
{
    public class UpdateMyProfileDto
    {
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public int Age { get; set; }
    }
}
