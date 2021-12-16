using CSS_Server.Models.Database.DBObjects;
using CSS_Server.Models.Database.Repositories;
using System.Security.Claims;

namespace CSS_Server.Models.Authentication
{
    public class BaseUser
    {
        private static readonly SQLiteRepository<DBUser> _repository = new SQLiteRepository<DBUser>();
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

        public int Id
        {
            get
            {
                Claim idClaim = _user.FindFirst(ClaimTypes.NameIdentifier);
                if (idClaim == null || !int.TryParse(idClaim.Value, out int id))
                    id = -1;
                return id;
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

        public User GetUserObject()
        {
            DBUser dBUser = _repository.Get(Id);
            if (dBUser == null)
                return null;
            return new User(dBUser);
        }
    }
}
