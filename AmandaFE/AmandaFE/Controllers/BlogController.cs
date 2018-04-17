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
            User user = await Cookies.GetUserFromCookie(Request, _context);

            PostCreateViewModel vm = new PostCreateViewModel()
            {
                UserName = user?.Name
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [Bind("UserName", "PostTitle", "PostContent")] PostCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["NotificationType"] = "alert-warning";
                TempData["NotificationMessage"] = "One or more of the blog fields were entered incorrectly. Please try again.";
                return View(vm);
            }

            vm.PostContent = vm.PostContent.Trim();

            User user = _context.User.FirstOrDefault(u => u.Name == vm.UserName);

            if (user is null)
            {
                user = new User() { Name = vm.UserName };
                await _context.AddAsync(user);
                // Make sure the new user is actually in the database before attributing the post to them
                await _context.SaveChangesAsync();
            }

            Post post = new Post()
            {
                Content = vm.PostContent,
                Summary = vm.PostContent.Substring(0, 20),
                CreationDate = DateTime.Now,
                Title = vm.PostTitle,
                User = user
            };

            // TODO(taylorjoshuaw): Add API calls to our custom backend API

            await _context.Post.AddAsync(post);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                TempData["NotificationType"] = "alert-danger";
                TempData["NotificationMessage"] = "Unable to commit new post to backend database. Please try again.";
                return View(vm);
            }

            TempData["NotificationType"] = "alert-success";
            TempData["NotificationMessage"] = $"Successfully posted {post.Title}!";

            // TODO(taylorjoshuaw): Will change this to viewing the post the user just created
            return RedirectToAction("Index");
        }
    }
}
