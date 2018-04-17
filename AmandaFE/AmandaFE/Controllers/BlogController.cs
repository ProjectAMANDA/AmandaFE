using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AmandaFE.Models;
using AmandaFE.Data;

namespace AmandaFE.Controllers
{
    public class BlogController : Controller
    {
        private readonly BlogDBContext _context;

        public BlogController(BlogDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            Post post = new Post();

            if (Request.Cookies.ContainsKey(Cookies.UserId) &&
                Int32.TryParse(Request.Cookies[Cookies.UserId], out int userId))
            {
                User user = _context.User.FirstOrDefault(u => u.Id == userId);
                post.UserId = userId;
                post.User = user;
            }
        }
    }
}
