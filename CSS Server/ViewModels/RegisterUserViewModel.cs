using Newtonsoft.Json.Linq;

namespace CSS_Server.ViewModels
{
    public class RegisterUserViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RetypePassword { get; set; }
        public bool SuccesfullAdded { get; set; }
        public JObject Errors { get; set; }
    }
}
