using System.ComponentModel.DataAnnotations;

namespace CSS_Server.ViewModels
{
    public class RegisterUserViewModel
    {
        [Required(ErrorMessage = "Fill in a username.")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "What a weird email you have. We're not going to do anything with this.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A Password is required!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Retype your password!")]
        [Compare("Password", ErrorMessage = "Confirm your password!")]
        public string RetypePassword { get; set; }
        public bool SuccesfullAdded { get; set; }
    }
}
