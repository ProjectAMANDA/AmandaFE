using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AmandaFE.Models;
using AmandaFE.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using Newtonsoft.Json.Linq;

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
            return View(_context.Post);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            User user = await Cookies.GetUserFromCookie(Request, _context);

            PostCreateViewModel vm = new PostCreateViewModel()
            {
                UserName = user?.Name,
                EnrichPost = true
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [Bind("UserName", "PostTitle", "PostContent", "EnrichPost")] PostCreateViewModel vm)
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

            if (!vm.EnrichPost)
            {
                TempData["NotificationType"] = "alert-success";
                TempData["NotificationMessage"] = $"Successfully posted {post.Title}!";
                return RedirectToAction("Details", new { post.Id });
            }

            return RedirectToAction("Enrich", new { post.Id });
        }

        public async Task<IActionResult> Enrich(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }

            Post post = await _context.Post.FirstOrDefaultAsync(p => p.Id == id);

            if (post is null)
            {
                TempData["NotificationType"] = "alert-warning";
                TempData["NotificationMessage"] = "Could not find the specified blog post.";
                return RedirectToAction("Index");
            }

            IEnumerable<string> imageHrefs;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:50063/api/");
                client.DefaultRequestHeaders.Add("text", post.Content);

                HttpResponseMessage response = await client.GetAsync("image");

                if (response.IsSuccessStatusCode)
                {
                    JArray apiArray = JArray.Parse(await response.Content.ReadAsStringAsync());
                    imageHrefs = apiArray.ToObject<string[]>();
                }
                else
                {
                    imageHrefs = new string[0];
                }
            }

            if (imageHrefs is null || imageHrefs.Count() < 1)
            {
                TempData["NotificationType"] = "alert-danger";
                TempData["NotificationMessage"] = "Could not reach remote enrichment services, but successfully created blog post. Please try enrichment services later.";
                return RedirectToAction("Details", new { post.Id });
            }

            PostEnrichViewModel vm = new PostEnrichViewModel()
            {
                Post = post,
                ImageHrefs = imageHrefs
            };

            return View(vm);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }

            Post post = await _context.Post.Include(p => p.User)
                                           .FirstOrDefaultAsync(p => p.Id == id.Value);

            if (post is null)
            {
                TempData["NotificationType"] = "alert-warning";
                TempData["NotificationMessage"] = "Could not find the specified blog post.";
                return RedirectToAction("Index");
            }

            return View(post);
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
    }
}
