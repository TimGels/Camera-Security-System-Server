using CSS_Server.Models;
using CSS_Server.Models.Database;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace CSS_Server.Controllers
{
    public class LogController : Controller
    {
        private readonly CSSContext _context;
        public LogController(CSSContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<Log> logs = _context.Logs.ToList();
            logs.Reverse();

            return View(logs);
        }
    }
}
