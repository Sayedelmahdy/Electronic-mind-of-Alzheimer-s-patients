using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.FamilyDto
{
    public class AssignPatientDto
    {
        [Required]
        public string PatientCode { get; set; }
        [Required]
        public string relationility {  get; set; }
    }
}
