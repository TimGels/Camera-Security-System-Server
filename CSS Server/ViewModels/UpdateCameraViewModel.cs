using System.ComponentModel.DataAnnotations;

namespace CSS_Server.ViewModels
{
    public class UpdateCameraViewModel
    {
        [Required(ErrorMessage = "Fill in a name for the camera.")]
        public string Name { get; set; }

        public string Description { get; set; }
        [DataType(DataType.Password)]

        public bool ChangePassword { get; set; }

        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Confirm your password!")]
        public string RetypePassword { get; set; }
    }
}
