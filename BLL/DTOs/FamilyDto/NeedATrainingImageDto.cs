using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BLL.DTOs.FamilyDto
{
    public class NeedATrainingImageDto
    {
        [JsonIgnore]
        public GlobalResponse GlobalResponse { get; set; }
        public bool NeedATraining { get; set; } 
        public List<string>? ImagesSamplesUrl { get; set; }
    }
}
