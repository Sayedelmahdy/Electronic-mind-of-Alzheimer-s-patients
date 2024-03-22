using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.AuthenticationDto
{
    public class EditUserDto
    {



        [StringLength(128)]
        public string? Email { get; set; }

        [StringLength(256)]
        public string? Password { get; set; }

        public IEnumerable<string> Role { get; set; }
        [StringLength(11)]
        public string? PhoneNumber { get; set; }

    }
}
