using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.PatientDto
{
    public class GetPatientProfileDto
    {
        public string PatientId { get; set; }
        public string FullName { get; set; }
        public int Age { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DiagnosisDate { get; set; }

        //for Test 
        public string Message { get; set; }
        public bool HasError { get; set; }

    }
}
