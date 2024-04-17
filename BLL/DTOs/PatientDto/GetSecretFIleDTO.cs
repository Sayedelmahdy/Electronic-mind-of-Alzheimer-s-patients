﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.PatientDto
{
    public class GetSecretFileDto
    {
        public string SecretId { get; set; }
        public string FileName { get; set; }
        public string File_Description { get; set; }
        public string? DocumentUrl { get; set; }
        public string? DocumentExtension { get; set; }
        public bool NeedToConfirm { get; set; }
    }
}
