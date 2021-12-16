using System.ComponentModel.DataAnnotations;

namespace CSS_Server.ViewModels
{
    public class LogInViewModel
    {
        [Required(ErrorMessage = "An username is required to login!")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "A password is required to login!")]
        public string Password { get; set; }
    }
}
