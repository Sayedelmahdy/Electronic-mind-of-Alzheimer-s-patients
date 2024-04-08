using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class GetFamilyLocationDto
    {
        [JsonIgnore]
        public int Code { get; set; }
        public string Message { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

    }
}
