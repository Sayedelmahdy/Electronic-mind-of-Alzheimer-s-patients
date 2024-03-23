using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.AuthenticationDto
{
    public class ResetPassword
    {
        public string Message { set; get; }
        public bool IsPasswordReset { set; get; }
    }
}
