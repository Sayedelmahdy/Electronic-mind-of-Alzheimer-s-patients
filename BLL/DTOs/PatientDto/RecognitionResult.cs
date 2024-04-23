using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BLL.DTOs.PatientDto
{
    public class RecognitionResult
    {
        [JsonIgnore]
        public GlobalResponse GlobalResponse { get; set; }
        public ICollection< PersonInImage> PersonsInImage { get; set; }
        public string ImageAfterResultUrl { get; set; }
    }
}
