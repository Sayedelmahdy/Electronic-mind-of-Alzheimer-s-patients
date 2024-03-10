using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        public string NewPassWord { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
    }
}
