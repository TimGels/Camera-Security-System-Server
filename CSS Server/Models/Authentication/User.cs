using CSS_Server.Models.Database.DBObjects;
using CSS_Server.Models.Database.Repositories;
using CSS_Server.Utilities;
using System;

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
            set
            {
                if (value == UserName)
                    return;
                _dbUser.UserName = value;
                _repository.Update(_dbUser);
            }
        }

        public string Email
        {
            get { return _dbUser.Email; }
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

        public static User CreateUser(string email, string userName, string password)
        {
            //create hash and salt for the given password.
            string hash = HashHelper.GenerateHash(password, out string salt);

            //create a new DBUser object with all the props.
            DBUser dbUser = new DBUser()
            {
                Email = email,
                UserName = userName,
                Password = hash,
                Salt = salt,
            };

            try
            {
                _repository.Insert(dbUser);
                return new User(dbUser);
            }
            catch (Exception)
            {
                //catch any exceptions that can happen because of a non unique emailadress.
                //And do something of course with it..
                return null;
            }
        }
    }
}
