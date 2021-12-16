using System.Security.Claims;

namespace CSS_Server.Models.Authentication
{
    public class BaseUser
    {
        private readonly ClaimsPrincipal _user;
        public string UserName
        {
            get
            {
                Claim nameClaim = _user.FindFirst(ClaimTypes.Name);
                if (nameClaim == null)
                    return "Unauthorized user";
                return nameClaim.Value;
            }
        }

        public string Id
        {
            get
            {
                Claim idClaim = _user.FindFirst(ClaimTypes.NameIdentifier);
                if (idClaim == null)
                    return string.Empty;
                return idClaim.Value;
            }
        }

        public string Email
        {
            get
            {
                return _user.FindFirst(ClaimTypes.Email).Value;
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return _user != null && _user.Identity != null && _user.Identity.IsAuthenticated;
            }
        }

        public BaseUser(ClaimsPrincipal user)
        {
            _user = user;
        }
    }
}
