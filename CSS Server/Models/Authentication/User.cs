using CSS_Server.Models.Database.DBObjects;
using CSS_Server.Models.Database.Repositories;
using CSS_Server.Utilities;
using System;

namespace CSS_Server.Models
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
        private DBUser _dbUser;
        private static readonly UserRepository _repository = new UserRepository();

        public int Id
        {
            get { return _dbUser.Id; }
            set { 
                _dbUser.Id = value;
                _repository.Update(_dbUser);
            }
        }

        public string UserName
        {
            get { return _dbUser.UserName; }
            set
            {
                _dbUser.UserName = value;
                _repository.Update(_dbUser);
            }
        }

        public string Email
        {
            get { return _dbUser.Email; }
            set
            {
                _dbUser.Email = value;
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
