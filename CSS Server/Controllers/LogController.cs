using CSS_Server.Models.Database.DBObjects;
using CSS_Server.Models.Database.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CSS_Server.Controllers
{
    public class LogController : Controller
    {
        private static readonly SQLiteRepository<DBLog> _repository  = new SQLiteRepository<DBLog>();

        [HttpGet]
        public IActionResult Index()
        {
            List<DBLog> logs = _repository.GetAll();
            logs.Reverse();

            return View(logs);
        }
    }
}
