using AmandaFE.Data;
using AmandaFE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmandaFE.Controllers
{
    public class UserController : Controller
    {
        private readonly Data.BlogDBContext _context;

        public UserController(BlogDBContext context)
        {
            _context = context;
        }

        // TODO(taylorjoshuaw): Change userName to Id from user table
        public async Task<IActionResult> Index(string userName)
        {
            if (userName == null)
            {
                return NotFound();
            }

            if(Request.Cookies[userName] != null)
            {
                var user = _context.User.Where(u => u.Name == userName);
            }

            return View(userName);
        }
    }
}
