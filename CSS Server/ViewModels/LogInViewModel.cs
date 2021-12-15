using System.ComponentModel.DataAnnotations;

namespace CSS_Server.ViewModels
{
    public class LogInViewModel
    {
        [Required(ErrorMessage = "An email is required to login!")]
        [EmailAddress(ErrorMessage = "What a weird email you have. We're not going to do anything with this.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A password is required to login!")]
        public string Password { get; set; }
    }
}
