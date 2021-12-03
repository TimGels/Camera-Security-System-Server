using CSS_Server.Models;
using CSS_Server.Models.Database.Repositories;
using CSS_Server.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CSS_Server.Controllers
{
    public class AccountController : Controller
    {
        private AuthenticationManager _authenticationManager;

        public AccountController(AuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(LogInViewModel form)
        {
            if (!ModelState.IsValid)
                return View(form);
            try
            {
                User user = new UserRepository().GetByEmail(form.Email);

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

    }
}
