using AmandaFE.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmandaFE.Controllers
{
    public class BlogController : Controller
    {
        private readonly BlogDBContext _context;

        public BlogController(BlogDBContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Find(string search)
        {
            var posts = from p in _context.Post 
                        select p;

            if (!String.IsNullOrEmpty(search))
            {
                // Search for a match under either Title or Keywords
                posts = posts.Where(s => (s.Title.Contains(search) || s.Keywords.Contains(search)));

            }

            return View(await posts.ToListAsync());
        }

        // Other blog display tasks here
    }
}
