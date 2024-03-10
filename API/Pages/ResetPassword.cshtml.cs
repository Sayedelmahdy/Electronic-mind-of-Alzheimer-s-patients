using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace API.Pages
{
    public class ResetPasswordModel : PageModel
    {
        [BindProperty]
        public string Token { get; set; }
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "New Password is required")]
        public string NewPassword { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Confirm Password is required")]
        public string ConfirmPassword { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            // Handle the GET request if needed
        }

        public IActionResult OnPost()
        {
            if (NewPassword != ConfirmPassword)
            {
                ErrorMessage = "Password and confirmation do not match.";
                return Page();
            }

            // Continue with the rest of your logic...

            // Redirect to a success page or return a different IActionResult as needed.
            return RedirectToPage("/Resetpassword");
        }
    }
}
