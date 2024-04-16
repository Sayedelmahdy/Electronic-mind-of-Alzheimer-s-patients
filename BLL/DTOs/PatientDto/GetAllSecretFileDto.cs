﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BLL.DTOs.PatientDto
{
    public class GetAllSecretFileDto
    {
        [JsonIgnore]
        public int Code { get; set; }
        public bool NeedToConfirm { get; set; }
        public IEnumerable<GetSecretFIleDTO> SecretFiles { get; set; }
    }
}
