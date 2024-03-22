using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BLL.DTOs.FamilyDto
{
    public class GetPatientProfile
    {

        [JsonIgnore] 
        public bool ErrorAppear { get; set; }
        [JsonIgnore]
        public string Message { get; set; }
        public string FullName { get; set; } 
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int Age { get; set; }
        public string relationality { get; set; }
        public string DiagnosisDate { get; set; }
    }
}
