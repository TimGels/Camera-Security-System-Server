using CSS_Server.Models;
using CSS_Server.Models.Authentication;
using CSS_Server.Models.Database.DBObjects;
using CSS_Server.Models.Database.Repositories;
using CSS_Server.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ApplicationUser = CSS_Server.Models.Authentication.User;

namespace CSS_Server.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AuthenticationManager _authenticationManager;
        private static readonly SQLiteRepository<DBUser> _repository = new SQLiteRepository<DBUser>();
        private static bool _needSetupAccount = _repository.GetAll().Count() < 1;

        public AccountController(ILogger<AccountController> logger, IServiceProvider provider)
        {
            _logger = logger;
            _authenticationManager = provider.GetRequiredService<AuthenticationManager>();
        }

        #region Login and Logout
        [HttpGet]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn(LogInViewModel form)
        {
            if (new BaseUser(User).IsAuthenticated)
                return RedirectToAction("Index", "Camera");

            if (_needSetupAccount)
                return RedirectToAction("Register");

            ViewData["Title"] = "CSS log-in";

            if (Request.Method == "POST")
                ViewData["AlreadyPosted"] = true;

            if (!ModelState.IsValid || Request.Method == "GET")
                return View(form);

            try
            {
                User user = new SQLiteRepository<DBUser>().GetByEmail(form.UserName);

                if(user != null && user.Validate(form.Password))
                {
                    await _authenticationManager.SignIn(HttpContext, user);
                    return RedirectToAction("Index", "Camera");
                }
                TempData["snackbar"] = "Email or password incorrect. Try again!";
                return View(form);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("summary", ex.Message);
                return View(form);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _authenticationManager.SignOut(this.HttpContext);
            return RedirectToAction("LogIn", "Account");
        }
        #endregion

        #region Endpoints for CRUD operations on users.
        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Title"] = "CSS: User overview";
            List<User> users = _repository.GetAll().Select(dbUser => new User(dbUser)).ToList();
            return View(users);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            // Get the current user
            BaseUser currentUser = new BaseUser(User);

            //Check if the user does not want to delete itself
            if(currentUser.Id == id)
                return BadRequest();

            //Remove the user from the database.
            _repository.Delete(id);

            //Log the deletion
            _logger.LogCritical("User with id:{0} deleted by user {1} ({2})", id, currentUser.UserName, currentUser.Id);

            return Ok();
        }

        [HttpGet]
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Register(RegisterUserViewModel form)
        {
            BaseUser currentUser = new BaseUser(User);

            //For an unauthorized user registering a new user is only possible when the needSetupAccount Bool == true
            if (!currentUser.IsAuthenticated && !_needSetupAccount)
                return RedirectToAction("LogIn");

            ViewData["Title"] = "CSS: Register user";

            if (Request.Method == "POST")
                ViewData["AlreadyPosted"] = true;

            if (!ModelState.IsValid || Request.Method == "GET")
                return View(form);

            ApplicationUser newUser = ApplicationUser.CreateUser(form.UserName, form.Password, out Dictionary<string, string> errors);
            if (newUser != null)
            {
                _logger.LogCritical("{0} ({1}) registered a new user {2} ({3})",
                    currentUser.UserName, currentUser.Id, newUser.UserName, newUser.Id);

                TempData["snackbar"] = "User was succesfully added!";

                if (currentUser.IsAuthenticated)
                 return RedirectToAction("Index");

                _needSetupAccount = false;
                return RedirectToAction("LogIn");
            }
            if(errors.Count > 0)
            {
                foreach(KeyValuePair<string, string> error in errors)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }
            }
            return View(form);
        }

        [HttpGet]
        [HttpPost]
        public IActionResult Update(UpdateUserViewModel form, int id)
        {
            //Get the camera with given id.
            DBUser dbUser = _repository.Get(id);

            //if no camera is found with given id, go to the camera overview.
            if (dbUser == null)
                return RedirectToAction("Index");

            User user = new User(dbUser);

            ViewData["userName"] = user.UserName;
            ViewData["Title"] = "CSS: Update user";

            //Fill in the form with the current values of the camera.
            if (Request.Method == "GET")
            {
                return View(form);
            }

            //From here handle the post:
            ViewData["AlreadyPosted"] = true;
            
            //TODO extra password validation!
            if (form.Password == null || form.Password == String.Empty)
                ModelState.AddModelError("Password", "You have to fill in a password");
            if (form.RetypePassword == null || form.RetypePassword == String.Empty || form.RetypePassword != form.Password)
                ModelState.AddModelError("RetypePassword", "You have to confirm your password correctly!");

            if (!ModelState.IsValid)
                return View(form);

            BaseUser currentUser = new BaseUser(User);

            user.Password = form.Password;
            _logger.LogCritical("User {0}, ({1}) has updated the password from user with id {2}", currentUser.UserName, currentUser.Id, user.Id);

            TempData["snackbar"] = "Changed password succesfully";
            return RedirectToAction("Index");
        }
        #endregion

    }
}
