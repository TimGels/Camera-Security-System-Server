using System.ComponentModel.DataAnnotations;

namespace CSS_Server.ViewModels
{
    public class UpdateUserViewModel
    {
        [Required(ErrorMessage = "Fill in a username.")]
        public string UserName { get; set; }
        public bool ChangePassword { get; set; }
        public string Password { get; set; }
        public string RetypePassword { get; set; }
    }
}
