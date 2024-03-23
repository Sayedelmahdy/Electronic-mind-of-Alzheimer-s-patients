using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.AuthenticationDto
{
    public class RegisterAuthDto
    {
        public string Message { get; set; }
        public bool NeedToConfirm { get; set; }

    }
}
