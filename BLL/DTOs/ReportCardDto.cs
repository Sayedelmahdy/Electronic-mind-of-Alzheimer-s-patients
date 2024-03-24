using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class ReportCardDto
    {
        
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
        public string ReportContent { get; set; } = string.Empty;
        
        public string patientid { get; set; }
    }
}
