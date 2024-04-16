using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.CaregiverDto
{
    public class ReportCardDto
    {
        [Required]
        public DateOnly FromDate { get; set; }
        [Required]
        public DateOnly ToDate { get; set; }
        [Required]
        public string ReportContent { get; set; } = string.Empty;

        [Required]
        public string patientid { get; set; }
    }
}
