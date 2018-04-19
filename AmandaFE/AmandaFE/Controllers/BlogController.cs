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
using System.Text.RegularExpressions;

namespace AmandaFE.Controllers
{
    public class BlogController : Controller
    {
        private readonly BlogDBContext _context;

        public BlogController(BlogDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchKeywordString,
            string searchUserName, int? page)
        {
            if (string.IsNullOrWhiteSpace(searchKeywordString) &&
                string.IsNullOrWhiteSpace(searchUserName))
            {
                return View(new PostIndexViewModel()
                {
                    Posts = await _context.Post.Include(p => p.User)
                                               .ToListAsync()
                });
            }

            PostIndexViewModel vm = new PostIndexViewModel()
            {
                SearchKeywordString = searchKeywordString,
                SearchUserName = searchUserName,
                Posts = new List<Post>()
            };

            if (!string.IsNullOrWhiteSpace(searchKeywordString))
            {
                vm.Posts = await KeywordUtilities.GetPostsByKeywordStringAsync(
                    searchKeywordString, _context);
            }

            // TODO(taylorjoshuaw): Add searching by username

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            User user = await Cookies.GetUserFromCookieAsync(Request, _context);

            PostCreateViewModel vm = new PostCreateViewModel()
            {
                UserName = user?.Name,
                EnrichPost = true
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [Bind("UserName", "PostTitle", "PostContent", "EnrichPost", "Keywords")] PostCreateViewModel vm)
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
                Summary = vm.PostContent.Substring(0, Math.Min(vm.PostContent.Length, 100)),
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

            await KeywordUtilities.MergeKeywordStringIntoPostAsync(vm.Keywords, post.Id, _context);
            await Cookies.WriteUserCookieByIdAsync(post.User.Id, Response, _context);

            if (!vm.EnrichPost)
            {
                TempData["NotificationType"] = "alert-success";
                TempData["NotificationMessage"] = $"Successfully posted {post.Title}!";
                return RedirectToAction("Details", new { post.Id });
            }

            return RedirectToAction("Enrich", new { post.Id });
        }

        [HttpGet]
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

            IEnumerable<string> imageHrefs = await BackendAPI.GetImageHrefs(post.Content);

            if (imageHrefs is null || imageHrefs.Count() < 1)
            {
                TempData["NotificationType"] = "alert-danger";
                TempData["NotificationMessage"] = "Could not reach remote enrichment services, but successfully created blog post. Please try enrichment services later.";
                return RedirectToAction("Details", new { post.Id });
            }

            PostEnrichViewModel vm = new PostEnrichViewModel()
            {
                Post = post,
                PostId = post.Id,
                ImageHrefs = imageHrefs
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Enrich(
            [Bind("PostId", "SelectedImageHref")] PostEnrichViewModel vm)
        {
            if (vm is null)
            {
                return RedirectToAction("Index");
            }

            Post post = await _context.Post.FirstOrDefaultAsync(p => p.Id == vm.PostId);

            if (post is null)
            {
                TempData["NotificationType"] = "alert-danger";
                TempData["NotificationMessage"] = "Could not find the specified post to enrich. Please try again.";
                return RedirectToAction("Enrich", new { vm.PostId });
            }

            post.ImageHref = vm.SelectedImageHref;
            _context.Post.Update(post);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                TempData["NotificationType"] = "alert-danger";
                TempData["NotificationMessage"] = $"Could not commit post enrichment to backend database. Please try again.";
                vm.ImageHrefs = await BackendAPI.GetImageHrefs(post.Content);
                vm.Post = post;

                return View(vm);
            }

            TempData["NotificationType"] = "alert-success";
            TempData["NotificationMessage"] = $"Successfully enriched {post.Title}";
            return RedirectToAction("Details", new { id = vm.PostId });
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }

            Post post = await _context.Post.Include(p => p.User)
                                           .Include(p => p.PostKeywords)
                                           .FirstOrDefaultAsync(p => p.Id == id.Value);

            if (post is null)
            {
                TempData["NotificationType"] = "alert-warning";
                TempData["NotificationMessage"] = "Could not find the specified blog post.";
                return RedirectToAction("Index");
            }

            PostDetailViewModel vm = new PostDetailViewModel()
            {
                Post = post,
                Keywords = await _context.PostKeyword.Include(pk => pk.Keyword)
                                                     .Where(pk => pk.PostId == post.Id)
                                                     .Select(pk => pk.Keyword)
                                                     .ToListAsync()

                /* This is how it will be done in the Edit action
                KeywordString = string.Join(", ", _context.PostKeyword.Include(pk => pk.Keyword)
                                                                      .Select(pk => pk.Keyword)
                                                                      .ToList())
                */
            };

            return View(vm);
        }

        public async Task<IActionResult> Find(string search)
        {
            var posts = from p in _context.Post
                        select p;

            if (!String.IsNullOrEmpty(search))
            {
                // Search for a match under either Title or Keywords
                //posts = posts.Where(s => (s.Title.Contains(search) || s.Keyword.Contains(search)));

            }

            return View(await posts.ToListAsync());
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.SingleOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,User,UserId,Title,Content")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            Post existingPost = await _context.Post.FirstOrDefaultAsync(p => p.Id == id);

            if (ModelState.IsValid && existingPost != null)
            {
                try
                {
                    // Update the fields in the existing post based on the relevant
                    // edited columns and update the date (might need a "ModifiedDate" in the future)
                    existingPost.Title = post.Title;
                    existingPost.Content = post.Content;
                    existingPost.CreationDate = DateTime.Now;

                    // TODO(taylorjoshuaw): Add tags here

                    _context.Update(existingPost);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    TempData["NotificationType"] = "alert-danger";
                    TempData["NotificationMessage"] = "Unable to commit changes to backend database. Please try again.";
                    return View(existingPost);
                }

                return RedirectToAction("Details", new { id });
            }

            return View(existingPost);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.SingleOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Post.SingleOrDefaultAsync(p => p.Id == id);
            _context.Post.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(p => p.Id == id);
        }
    }
}
