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
        public async Task<IActionResult> Index(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            if(Request.Cookies[id.ToString()] != null)
            {
                try
                {
                    var user = await _context.User.FirstAsync(u => u.Id == id.Value);
                    return View(user);
                }
                catch
                {
                    return NotFound();
                }
            }

            return View();
        }

        public async Task<IActionResult> Details(int? id)
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
                return RedirectToAction(nameof(Index));
            }
            return View(user);
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

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmation(int id)
        {
            var user = await _context.User.FirstAsync(u => u.Id == id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(u => u.Id == id);
        }

    }
}
