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

        /// <summary>
        /// Handles the user index page view, displays all posts by the current
        /// logged in user
        /// </summary>
        /// <returns>View with UserViewModel</returns>
        public async Task<IActionResult> Index()
        {
            UserViewModel vm = new UserViewModel();
            try
            {
                vm.User = await Cookies.GetUserFromCookieAsync(Request, _context);

                vm.AllPosts = vm.User.Posts;
                return View(vm);
            }
            catch
            {
                return View();
            }
            

        }

        /// <summary>
        /// Currently not in use. Was kept to flush out more when we
        /// add more information to the user model
        /// </summary>
        /// <param name="id">Id of user from DB</param>
        /// <returns>Nothing at this time</returns>
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

        /// <summary>
        /// Currently not in use, we are creating the User through the creation of
        /// a blog post. Kept so that we can flush it out and make a better user
        /// experience in the future
        /// </summary>
        /// <param name="user">An instance of a User</param>
        /// <returns>Nothing at this time</returns>
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

        /// <summary>
        /// Currently not in use, there really isn't much to edit on the user. We
        /// were not able to get to flushing out the user model.
        /// </summary>
        /// <param name="id">Id of User from DB</param>
        /// <returns>Nothing at this time</returns>
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

        /// <summary>
        /// Currently not in use, there really isn't much to edit on the user. We
        /// were not able to get to flushing out the user model.
        /// </summary>
        /// <param name="id">Id of user</param>
        /// <param name="user">Instance of a User</param>
        /// <returns>Nothing at this time</returns>
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

        /// <summary>
        /// Currently not in use.
        /// </summary>
        /// <param name="id">Id of user</param>
        /// <returns>nothing at this time</returns>
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

        /// <summary>
        /// Currently not in use
        /// </summary>
        /// <param name="id">Id of user</param>
        /// <returns>Nothing at this time</returns>
        [HttpDelete, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmation(int id)
        {
            var user = await _context.User.FirstAsync(u => u.Id == id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Checks if there is a user with the passed in ID in the database
        /// </summary>
        /// <param name="id">Id of user</param>
        /// <returns>boolean, true for exists and false if not</returns>
        private bool UserExists(int id)
        {
            return _context.User.Any(u => u.Id == id);
        }

    }
}
