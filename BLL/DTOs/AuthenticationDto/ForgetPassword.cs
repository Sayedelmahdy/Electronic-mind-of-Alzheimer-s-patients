﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.AuthenticationDto
{
    public class ForgetPassword
    {
        public string Message { set; get; }
        public bool IsEmailSent { set; get; }
    }
}
