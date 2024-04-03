using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.PatientDto
{
    public class GetMediaforPatientDto
    {
        public string MediaId { get; set; }
        public DateTime Uploaded_date { get; set; }
        public string Caption { get; set; }
        public string MediaUrl { get; set; }
        public string MediaExtension { get; set; }
        public string FamilyNameWhoUpload { get; set; }
    }
}
