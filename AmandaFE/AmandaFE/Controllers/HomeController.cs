using AmandaFE.Data;
using AmandaFE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IActionResult> Index()
        {
            return View(new PostIndexViewModel()
            {
                Posts = await _context.Post.Include(p => p.User)
                                            .ToListAsync(),
                PostKeywords = _context.PostKeyword
            });
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
