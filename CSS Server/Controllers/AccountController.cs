using CSS_Server.Models.Authentication;
using CSS_Server.Models.Database;
using CSS_Server.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSS_Server.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        public static bool needSetupAccount;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly CSSContext _context;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, 
            ILogger<AccountController> logger, CSSContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        #region Login and Logout
        [HttpGet]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn(LogInViewModel form)
        {
            if (new BaseUser(User).IsAuthenticated)
                return RedirectToAction("Index", "Camera");

            if (needSetupAccount)
                return RedirectToAction("Register");

            ViewData["Title"] = "CSS log-in";

            if (Request.Method == "POST")
                ViewData["AlreadyPosted"] = true;

            if (!ModelState.IsValid || Request.Method == "GET")
                return View(form);

            var result = await _signInManager.PasswordSignInAsync(form.UserName, //TODO username must be email.
                           form.Password, false, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Camera");
            }

            if (result.IsLockedOut)
            {
                TempData["snackbar"] = "You are locked out!";
                return View(form);
            }

            TempData["snackbar"] = "Email or password incorrect. Try again!";
            return View(form);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("LogIn", "Account");
        }
        #endregion

        #region Endpoints for CRUD operations on users.
        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Title"] = "CSS: User overview";
            List<User> users = _context.Users.ToList();
            return View(users);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            // Get the current user
            BaseUser currentUser = new BaseUser(User);

            //Check if the user does not want to delete itself
            if (currentUser.Id == id)
                return BadRequest();

            //Remove the user with the usermanager
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user);

            //Log the deletion
            _logger.LogCritical("User with id:{0} deleted by user {1} ({2})", id, currentUser.UserName, currentUser.Id);

            return Ok();
        }

        [HttpGet]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterUserViewModel form)
        {
            BaseUser currentUser = new BaseUser(User);

            //For an unauthorized user registering a new user is only possible when the needSetupAccount Bool == true
            if (!currentUser.IsAuthenticated && !needSetupAccount)
                return RedirectToAction("LogIn");

            ViewData["Title"] = "CSS: Register user";

            if (Request.Method == "POST")
                ViewData["AlreadyPosted"] = true;

            if (!ModelState.IsValid || Request.Method == "GET")
                return View(form);

            var user = new User { UserName = form.UserName, Email = form.UserName };
            var result = await _userManager.CreateAsync(user, form.Password);

            if (result.Succeeded)
            {
                _logger.LogCritical("{0} ({1}) registered a new user",
                    currentUser.UserName, currentUser.Id);

                TempData["snackbar"] = "User was succesfully added!";

                if (currentUser.IsAuthenticated)
                    return RedirectToAction("Index");

                needSetupAccount = false;
                return RedirectToAction("LogIn");
            }
            if(result.Errors.Count() > 0)
            {
                foreach(var error in result.Errors)
                {
                    if(error.Code == "DuplicateUserName")
                        ModelState.AddModelError("UserName", error.Description);
                    else
                        ModelState.AddModelError("Password", error.Description);
                }
            }
            TempData["snackbar"] = "Somethin went wrong while registering the user.";
            return View(form);
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Update(UpdateUserViewModel form, string id)
        {
            //get user from usermanager
            var user = await _userManager.FindByIdAsync(id);

            //if no user is found with given id, go to the camera overview.
            if (user == null)
                return RedirectToAction("Index");

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

            //create a reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            //change the password
            var result = await _userManager.ResetPasswordAsync(user, token, form.Password);

            if (result.Succeeded)
            {
                BaseUser currentUser = new BaseUser(User);
                _logger.LogCritical("User {0}, ({1}) has updated the password from user with id {2}", currentUser.UserName, currentUser.Id, user.Id);
                TempData["snackbar"] = "Changed password succesfully";
            } else
            {
                TempData["snackbar"] = "Password not changed successfully";
            }

            return RedirectToAction("Index");
        }
        #endregion
    }
}
