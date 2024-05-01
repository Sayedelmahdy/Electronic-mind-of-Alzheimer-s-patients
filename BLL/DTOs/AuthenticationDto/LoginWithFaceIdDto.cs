using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.AuthenticationDto
{
    public class LoginWithFaceIdDto
    {
        public IFormFile Image { get; set; }

    }
}
