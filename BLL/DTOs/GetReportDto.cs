using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class GetReportDto
    {
        public string ReportId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ReportContent { get; set; } = string.Empty;

    }
}
