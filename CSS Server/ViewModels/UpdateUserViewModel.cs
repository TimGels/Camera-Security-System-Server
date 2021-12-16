using System.ComponentModel.DataAnnotations;

namespace CSS_Server.ViewModels
{
    public class UpdateUserViewModel
    {
        public string Password { get; set; }
        public string RetypePassword { get; set; }
    }
}
