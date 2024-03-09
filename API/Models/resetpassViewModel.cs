using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class resetpassViewModel
    {
        public string email { get; set; }
        public string token { get; set; }
        [Required]
        public string NewPassWord { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
    }
}
