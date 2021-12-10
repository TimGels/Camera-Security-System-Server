using CSS_Server.Models.Database.DBObjects;
using CSS_Server.Models.Database.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CSS_Server.Models.Authentication
{
    public class BaseUser
    {
        private static readonly SQLiteRepository<DBUser> _repository = new SQLiteRepository<DBUser>();
        private readonly HttpContext _httpContext;
        public string UserName
        {
            get
            {
                return _httpContext.User.FindFirst(ClaimTypes.Name).Value;
            }
        }

        public int Id
        {
            get
            {
                if (!int.TryParse(_httpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, out int id))
                    id = -1;
                return id;
            }
        }

        public string Email
        {
            get
            {
                return _httpContext.User.FindFirst(ClaimTypes.Email).Value;
            }
        }

        public BaseUser(HttpContext httpContext)
        {
            _httpContext = httpContext;
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
