using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class ChangePassword
    {
        public string Message { get; set; }
        public bool PasswordIsChanged { get; set; }
        public bool ErrorAppear { get; set; }
    }
}
