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
        public IActionResult Index()
        {
            return RedirectToAction("Details");
        }

        public async Task<IActionResult> Details(int? id)
        {
            UserViewModel vm = new UserViewModel();

            if (id.HasValue)
            {
               vm.User = await _context.User.Include(u => u.Posts).FirstAsync(u => u.Id == id.Value);
            }
            else
            {
                vm.User = await Cookies.GetUserFromCookieAsync(Request, _context);
            }

            if(vm.User == null)
            {
                TempData["NotificationType"] = "alert-danger";
                TempData["NotificationMessage"] = "Specified user was not found";
                return RedirectToAction("Index", "Home");
            }

            // TODO(taylorjoshuaw): Flesh out the user details view more. Temporarily just search on user
            return RedirectToAction("Index", "Blog", new { searchUserName = vm.User.Name });
            /*
            vm.LastFive = vm.User.Posts.TakeLast(5);

            return View(vm);
            */
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Name")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details");
            }
            return RedirectToAction("Details");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var user = await _context.User.FirstAsync(u => u.Id == id.Value);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }

                }

                return RedirectToAction("Index");
            }

            return View(user);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var user = await _context.User.FirstAsync(u => u.Id == id.Value);

            if( user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpDelete, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmation(int id)
        {
            var user = await _context.User.FirstAsync(u => u.Id == id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(u => u.Id == id);
        }

    }
}
