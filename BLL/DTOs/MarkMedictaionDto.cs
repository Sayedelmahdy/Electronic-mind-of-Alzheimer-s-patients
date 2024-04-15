using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class MarkMedictaionDto
    {
        [Required]
        public string MedictaionId { get; set; }
        [Required]
        public bool IsTaken { get; set; }
       
    }
}
