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
using System.Threading.Tasks;

namespace CSS_Server.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AuthenticationManager _authenticationManager;

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
            return View();
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            return Ok();
        }

        [HttpGet]
        [HttpPost]
        public IActionResult Register()
        {
            return View();
        }
        #endregion
    }
}
