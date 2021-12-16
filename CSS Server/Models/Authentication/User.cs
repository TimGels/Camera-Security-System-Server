using CSS_Server.Models.Database.DBObjects;
using CSS_Server.Models.Database.Repositories;
using CSS_Server.Utilities;
using SQLite;
using System.Collections.Generic;

namespace CSS_Server.Models.Authentication
{
    public class User
    {
        #region Constructors
        public User(DBUser dbUser)
        {
            _dbUser = dbUser;
        }
        #endregion

        #region Properties
        private readonly DBUser _dbUser;
        private static readonly SQLiteRepository<DBUser> _repository = new SQLiteRepository<DBUser>();

        public int Id
        {
            get { return _dbUser.Id; }
        }

        public string UserName
        {
            get { return _dbUser.UserName; }
        }

        /// <summary>
        /// property setter for the Password of the user.
        /// A plain text string can be given as value. This setter will create a hash and a salt.
        /// After setting, the values will be updated in the database.
        /// </summary>
        public string Password
        {
            set {
                //create hash and salt for the given password.
                _dbUser.Password = HashHelper.GenerateHash(value, out string salt);
                _dbUser.Salt = salt;
                _repository.Update(_dbUser);
            }
        }


        #endregion

        /// <summary>
        /// Validates the given password to the saved hash in the database.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Validate(string password)
        {
            return HashHelper.Verify(password, _dbUser.Password, _dbUser.Salt);
        }

        public static User CreateUser(string userName, string password, out Dictionary<string, string> errors)
        {
            //create hash and salt for the given password.
            string hash = HashHelper.GenerateHash(password, out string salt);

            //create a new DBUser object with all the props.
            DBUser dbUser = new DBUser()
            {
                UserName = userName,
                Password = hash,
                Salt = salt,
            };

            try
            {
                _repository.Insert(dbUser);
                errors = null;
                return new User(dbUser);
            }
            //catch exception that can happen because of a non unique userName.
            catch (SQLiteException ex) when (ex.Result == SQLite3.Result.Constraint && ex.Message == "UNIQUE constraint failed: User.userName")
            {
                errors = new Dictionary<string, string>();
                errors.Add("UserName", "Username must be unique, there is already another user with this username");
                return null;
            }
        }
    }
}
