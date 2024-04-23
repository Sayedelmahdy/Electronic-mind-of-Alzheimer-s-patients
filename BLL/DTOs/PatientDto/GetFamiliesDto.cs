using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.PatientDto
{
    public class GetFamiliesDto
    {
        public string FamilyId { get; set; }
        public string FamilyName { get; set; }
        public string? Relationility { get; set; }
        public string? FamilyDescriptionForPatient { get; set; }
        public string? HisImageUrl { get; set; }
        
    }
}
