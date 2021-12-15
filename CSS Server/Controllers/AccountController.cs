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
            if (!ModelState.IsValid || Request.Method == "GET")
            {
                ViewData["Title"] = "CSS log-in";
                ViewData["Page"] = "login";
                return View(form);
            }
            try
            {
                User user = new SQLiteRepository<DBUser>().GetByEmail(form.Email);

                if(user != null && user.Validate(form.Password))
                    await _authenticationManager.SignIn(this.HttpContext, user);

                return RedirectToAction("Index", "Camera", null);
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
            _logger.LogInformation("User with id:{0} deleted by user {1} ({2})", id, currentUser.UserName, currentUser.Id);

            return Ok();
        }

        [HttpGet]
        [HttpPost]
        public IActionResult Register(RegisterUserViewModel form)
        {
            if (Request.Method == "POST")
                ViewData["AlreadyPosted"] = true;

            if (!ModelState.IsValid || Request.Method == "GET")
                return View(form);

            ApplicationUser newUser = ApplicationUser.CreateUser(form.Email, form.UserName, form.Password);
            if (newUser != null)
            {
                BaseUser currentUser = new BaseUser(User);
                _logger.LogInformation("{0} ({1}) registered a new user {2} ({3}) with email {4}",
                    currentUser.UserName, currentUser.Id, newUser.UserName, newUser.Id, newUser.Email);
                form.SuccesfullAdded = true;
            }
            return View(form);
        }
        #endregion
    }
}
