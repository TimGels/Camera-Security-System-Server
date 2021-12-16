using System.ComponentModel.DataAnnotations;

namespace CSS_Server.ViewModels
{
    public class RegisterUserViewModel
    {
        [Required(ErrorMessage = "Fill in a username.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "A Password is required!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Retype your password!")]
        [Compare("Password", ErrorMessage = "Confirm your password!")]
        public string RetypePassword { get; set; }
    }
}
