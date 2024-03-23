using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class GlobalResponse
    {
        public string message {  get; set; }
        [JsonIgnore]
        public bool HasError { get; set; }
    }
}
