
using System.ComponentModel.DataAnnotations;

namespace CSS_Server.ViewModels
{
    public class RegisterCameraViewModel
    {
        [Required(ErrorMessage = "Fill in a name for the camera.")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "A Password is required!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Retype your password!")]
        [Compare("Password", ErrorMessage = "Confirm your password!")]
        public string RetypePassword { get; set; }

        public bool SuccesfullAdded { get; set; }
    }
}
