using AmandaFE.Data;
using AmandaFE.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AmandaFE.Controllers
{
    public class HomeController : Controller
    {
        private readonly Data.BlogDBContext _context;

        public HomeController(BlogDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            HomeViewModel vm = new HomeViewModel();

            vm.LastTen = _context.Post.TakeLast(10);

            return View(vm);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
