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

        /// <summary>
        /// Default action for the BlogController: GET /Blog
        /// Passes an instance of the PostIndexViewModel with Blog references
        /// based on the provided filtering critera to the Index.cshtml view
        /// </summary>
        /// <param name="searchKeywordString">Comma-separated string containing
        /// tokens to match to the Text property of rows within the Keyword table</param>
        /// <param name="searchUserName">Search string to match against the Name property
        /// of the User table</param>
        /// <param name="page">Optionally specified which page of posts to view. This
        /// parameter currently has no effect on the view model.</param>
        /// <returns>Asynchronous task for a ViewResult of the Index view provided
        /// with a populated PostIndexViewModel object</returns>
        public async Task<IActionResult> Index(string searchKeywordString,
            string searchUserName, int? page)
        {
            // If the serachKeywordString and searchUserName parameters are blank
            // or null, then just return a view for all blog posts with no filtering
            if (string.IsNullOrWhiteSpace(searchKeywordString) &&
                string.IsNullOrWhiteSpace(searchUserName))
            {
                return View(new PostIndexViewModel()
                {
                    Posts = await _context.Post.Include(p => p.User)
                                               .ToListAsync(),
                    PostKeywords = _context.PostKeyword
                });
            }

            // Instantiate a new view model based on the provided filtering criteria
            PostIndexViewModel vm = new PostIndexViewModel()
            {
                SearchKeywordString = searchKeywordString,
                SearchUserName = searchUserName,
                Posts = new List<Post>(),
                PostKeywords = _context.PostKeyword
            };

            // If keyword filtering has been provided then retrieve posts matching those
            // keywords using a union combination
            if (!string.IsNullOrWhiteSpace(searchKeywordString))
            {
                vm.Posts = await KeywordUtilities.GetPostsByKeywordStringAsync(
                    searchKeywordString, _context);
            }
            // If a partial username has been provided as a filtering criterion then either
            // union join with the keyword results if there are any or just retrieve all posts
            // partially matching the specified username filter
            if (!string.IsNullOrWhiteSpace(searchUserName))
            {
                if (vm.Posts.Any())
                {
                    vm.Posts = await vm.Posts.AsParallel()
                                             .Where(p => p.User.Name.Contains(searchUserName))
                                             .ToAsyncEnumerable()
                                             .ToList();
                }
                else
                {
                    vm.Posts = await _context.Post.Include(p => p.User)
                                                  .Include(p => p.PostKeywords)
                                                  .Where(p => p.User.Name.Contains(searchUserName))
                                                  .ToListAsync();
                }
            }

            return View(vm);
        }

        /// <summary>
        /// GET /Blog/Create
        /// Prepare a PostCreateViewModel object to pass to the Create.cshtml view used
        /// to create a new blog post and conditionally a new user within the database.
        /// If the user has already created a post on ProjectAMANDA, then the UserName
        /// property of the view model will be populated with the user name taken from
        /// the database. The form data contained within the rendered view will be posted
        /// to the POST action for this route.
        /// </summary>
        /// <returns>An asynchronous task of a ViewResult given a new PostCreateViewModel
        /// view model.</returns>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Try to get the User entity matching the user's stored ID stored as a cookie.
            // Returns null if no cookie was found.
            User user = await Cookies.GetUserFromCookieAsync(Request, _context);

            PostCreateViewModel vm = new PostCreateViewModel()
            {
                UserName = user?.Name,
                EnrichPost = true
            };

            return View(vm);
        }

        /// <summary>
        /// POST /Blog/Create
        /// </summary>
        /// <param name="vm">An instance of the PostCreateViewModel bound from the form data entered
        /// by the user in the Create.cshtml view. If the model state is invalid, the user is again
        /// presented with the Create.cshtml view with validation responses for the user to correct.
        /// 
        /// On success, this action will create a new Post entity within the Post table. Keyword
        /// entities will be added only if they do not already exist. All Keyword entities will be
        /// associated with the new post via a new PostKeyword junction table row in the database.
        /// Parallel Dots' discovered keywords are included in this action until the option to opt-out
        /// is implemented in the PostCreateViewModel and the Create.cshtml view.</param>
        /// <returns>Asynchronous task of either the Details or Enrich RedirectActionResult on success 
        /// depending on whether the user has chosen to opt-in or opt-out of third party API enrichment.
        /// If the model state was invalid, then this is simply a ViewResult for the Create view with
        /// the supplied view model and validation response.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("UserName", "PostTitle", "PostContent", "EnrichPost", "Keywords")] PostCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                /*
                TempData["NotificationType"] = "alert-warning";
                TempData["NotificationMessage"] = "One or more of the blog fields were entered incorrectly. Please try again.";
                */
                return View(vm);
            }

            // Prevent extraneous whitespace from being included in the database
            vm.PostContent = vm.PostContent.Trim();

            // If the user does not already exist, then create a new user and commit them to the database.
            // Otherwise, use the first User row that matches the provided username.
            User user = _context.User.FirstOrDefault(u => u.Name == vm.UserName);

            if (user is null)
            {
                user = new User() { Name = vm.UserName };
                await _context.AddAsync(user);
                // Make sure the new user is actually in the database before attributing the post to them
                await _context.SaveChangesAsync();
            }

            // Instantiate a new post based on the information provided via the form data-bound view model
            Post post = new Post()
            {
                Content = vm.PostContent,
                Summary = vm.PostContent.Substring(0, Math.Min(vm.PostContent.Length, 250)),
                CreationDate = DateTime.Now,
                Title = vm.PostTitle,
                User = user
            };

            // Add the post to the context's addition queue and try to commit it to the database
            await _context.Post.AddAsync(post);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                /*
                TempData["NotificationType"] = "alert-danger";
                TempData["NotificationMessage"] = "Unable to commit new post to backend database. Please try again.";
                */

                // Unable to commit the changes to the database. Allow the user another opportunity to add the post
                return View(vm);
            }

            // Create new Keyword entites if they do not exist yet and re-use the ones that already exist.
            // Create the needed PostKeyword junction tables related to the individual keywords and this new post.
            await KeywordUtilities.MergeKeywordStringIntoPostAsync(vm.Keywords, post.Id, _context);

            // Write the user's ID to the user's browser as a cookie to be used the next time GET /Blog/Create or
            // any other controller / action needs to retrieve the current user
            await Cookies.WriteUserCookieByIdAsync(post.User.Id, Response, _context);

            // If the user has opted-out of enriching their post, then simply redirect to the Details action for
            // their new post; otherwise, redirect to the Enrich action provided the new post's ID
            if (!vm.EnrichPost)
            {
                /*
                TempData["NotificationType"] = "alert-success";
                TempData["NotificationMessage"] = $"Successfully posted {post.Title}!";
                */
                return RedirectToAction("Details", new { post.Id });
            }

            return RedirectToAction("Enrich", new { post.Id });
        }

        /// <summary>
        /// GET /Blog/Enrich/id?
        /// Reaches out to the ProjectAMANDA REST API to retrieve all images
        /// related to the specified post by sentiment and most significant keyword.
        /// The images are provided to the Enrich.cshtml view via a new instance of
        /// the PostEnrichViewModel class along with metadata related to the images
        /// and post in regards to the sentiment and keyword matching criteria.
        /// </summary>
        /// <param name="id">The post id to enrich. If this is not provided, then
        /// this action will simply redirect to GET /Blog</param>
        /// <returns>Asynchronous task of a RedirectActionResult to Index upon an
        /// empty id or non-existing id. Redirects to the post's Details view if
        /// the enrichment services could not be reached for any reason (or if a
        /// blank / malformed response is provided by the REST API). Returns a
        /// ViewResult for the Enrich view upon successful retrieval of images,
        /// image metadata, and post metadata.</returns>
        [HttpGet]
        public async Task<IActionResult> Enrich(int? id)
        {
            // Redirect to index if no post id was specified
            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }

            Post post = await _context.Post.FirstOrDefaultAsync(p => p.Id == id);

            // If the provided post id does not exist, then just redirect the user to Index
            if (post is null)
            {
                /*
                TempData["NotificationType"] = "alert-warning";
                TempData["NotificationMessage"] = "Could not find the specified blog post.";
                */
                return RedirectToAction("Index");
            }

            // Get the first PostKeyword relationship row for the specified post if it exists to be used
            // as the "most significant phrase"
            PostKeyword postKeyword = await _context.PostKeyword.Include(pk => pk.Keyword)
                                                                .FirstOrDefaultAsync(pk => pk.PostId == post.Id);

            // Query the ProjectAMANDA REST API on the /api/analytics backend for this post's sentiment-matched
            // images and metadata
            JObject apiResponse = await BackendAPI.GetAnalyticsAsync(post.Content);

            // If the /api/analytics endpoint could not be reached or has provided a malformed response, then
            // gracefully redirect to the post's details view rather than erroring out
            if (apiResponse is null)
            {
                /*
                TempData["NotificationType"] = "alert-danger";
                TempData["NotificationMessage"] = "Could not reach remote enrichment services, but successfully created blog post. Please try enrichment services later.";
                */
                return RedirectToAction("Details", new { post.Id });
            }
            
            // The analytics endpoint was sucessfully reached! Extract the provided images from JSON into
            // a JToken list to simplify union joining with Bing image results
            List<JToken> images = apiResponse["images"].ToList();

            // If a "most significant keyword" exists in the database, then use it to get back keyword-associated
            // images using the ProjectAMANDA REST API's /api/image endpoint.
            if (postKeyword != null)
            {
                JObject bingResponse = await BackendAPI.GetBingAsync(postKeyword.Keyword.Text);
                images.AddRange(bingResponse["images"]);
            }

            // Compose all of the needed data for the user to enrich their post into a PostEnrichViewModel
            // for the Enrich.cshtml view
            PostEnrichViewModel vm = new PostEnrichViewModel()
            {
                Post = post,
                PostId = post.Id,
                Images = images.ToArray(),
                Sentiment = apiResponse["sentiment"].Value<float>(),
                SignificantPhrase = postKeyword.Keyword.Text
            };

            return View(vm);
        }

        /// <summary>
        /// POST /Blog/Enrich
        /// Uses the provided SelectedImageHref and PostId form responses from the GET /Blog/Enrich
        /// view form data to update the specified post's selected enrichment image in the database.
        /// </summary>
        /// <param name="vm">A PostEnrichViewModel instance using bound PostId and SelectedImageHref
        /// properties from the Enrich.cshtml view's form data.</param>
        /// <returns>Asyncronous task for a RedirectToActionResult to Index if the view model was
        /// not sucessfully bound, to the Enrich GET action if the post was not found in the database,
        /// or to details on success or if the changes could not be committed to the database.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enrich(
            [Bind("PostId", "SelectedImageHref")] PostEnrichViewModel vm)
        {
            // If the ViewModel is not valid, just reuturn the user to the index
            if (vm is null)
            {
                return RedirectToAction("Index");
            }

            // Try to find the first instance of the specified PostId. Redirect back to the initial enrich view
            // if the specified post was not found. TODO(taylorjoshuaw): This should redirect to Index, not Enrich.
            //                                                           If the post doesn't exist, why try again to enrich?
            Post post = await _context.Post.FirstOrDefaultAsync(p => p.Id == vm.PostId);

            if (post is null)
            {
                /* TempData crashes CLR on Azure *shrug*
                TempData["NotificationType"] = "alert-danger";
                TempData["NotificationMessage"] = "Could not find the specified post to enrich. Please try again.";
                */
                return RedirectToAction("Enrich", new { vm.PostId });
            }

            // Overwrite just the ImageHref field of the existing Post entity to ensure all other properties are preserved
            post.ImageHref = vm.SelectedImageHref;
            _context.Post.Update(post);

            // Try to commit the changes. If a database exception occurs, then just gracefully redirect to the post's details view
            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                /* TempData crashes CLR on Azure *shrug*
                TempData["NotificationType"] = "alert-danger";
                TempData["NotificationMessage"] = $"Could not commit post enrichment to backend database. Please try again later.";
                */
                return RedirectToAction("Details", new { id = vm.PostId });
            }

            /* TempData crashes CLR on Azure *shrug*
            TempData["NotificationType"] = "alert-success";
            TempData["NotificationMessage"] = $"Successfully enriched {post.Title}";
            */
            // Success! Show the user their newly enriched post
            return RedirectToAction("Details", new { id = vm.PostId });
        }

        /// <summary>
        /// GET /Blog/Details/id?
        /// </summary>
        /// <param name="id">The id of the post to view the details and associated image for</param>
        /// <returns>Asynchronous task of a ViewResult for the Details.cshtml view of the specified
        /// post. RedirectToActionResult to Index if the id was not provided or cannot be found in
        /// the database.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            // If no id is provided then simply redirect to index
            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }

            // Try to find the first post that matches the provided id, joining the result with
            // it navigational properties of User and PostKeywords to be used in the view model
            Post post = await _context.Post.Include(p => p.User)
                                           .Include(p => p.PostKeywords)
                                           .FirstOrDefaultAsync(p => p.Id == id.Value);

            // If the post could not be found, gracefully redirect to the Index action
            if (post is null)
            {
                /*
                TempData["NotificationType"] = "alert-warning";
                TempData["NotificationMessage"] = "Could not find the specified blog post.";
                */
                return RedirectToAction("Index");
            }

            // Instantiate a new PostDetailViewModel containing the post itself along with all keywords
            // associated with the post via matching rows of the PostKeyword table.
            PostDetailViewModel vm = new PostDetailViewModel()
            {
                Post = post,
                Keywords = await _context.PostKeyword.Include(pk => pk.Keyword)
                                                     .Where(pk => pk.PostId == post.Id)
                                                     .Select(pk => pk.Keyword)
                                                     .ToListAsync()
            };

            return View(vm);
        }

        /// <summary>
        /// GET /Blog/Find?search=xyz
        /// Deprecated action for performing blog post searches. Use the Index action
        /// with arguments for the username and phrase to search for instead. This method
        /// would be removed were it not for the code freeze.
        /// </summary>
        /// <param name="search">String to search for</param>
        /// <returns>ViewResult for the matching posts. Note: No Find view has been created!</returns>
        [Obsolete("Use the Index action to perform keyword and username searches")]
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

        /// <summary>
        /// GET /Blog/Edit/id?
        /// Prepares the Edit.cshtml view to edit the specified post. If no id was specified
        /// or the post could not be found, the user is given a 404 response. Otherwise, the
        /// Edit.cshtml view is given the matching Post entity, returning a 200 status code.
        /// </summary>
        /// <param name="id">The id for the post to render an Edit view for.</param>
        /// <returns>NotFoundResult if the id is empty or not found. ViewResult for the
        /// Edit.cshtml view with the specified post provided for the view's model on
        /// success.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            // If no id was provided, return a 404
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.SingleOrDefaultAsync(p => p.Id == id);

            // If the post is not found in the database, return a 404
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        /// <summary>
        /// POST /Blog/Edit
        /// </summary>
        /// <param name="id"></param>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,User,UserId,Title,Content")] Post post)
        {
            // If the requested post id does not match the bound Post model, then return a 404
            // TODO(taylorjoshuaw): This should be a BadRequest rather than a NotFound!
            if (id != post.Id)
            {
                return NotFound();
            }

            // Try to find the first post matching the specified id in the database
            Post existingPost = await _context.Post.FirstOrDefaultAsync(p => p.Id == id);

            // If the model state is valid and the post is found then overwrite the existing post's
            // properties with those from the Edit view's model.
            if (ModelState.IsValid && existingPost != null)
            {
                try
                {
                    // Update only the relevant fields in the existing post based on the 
                    // edited columns and update the date (might need a "ModifiedDate"
                    // column in the future)
                    existingPost.Title = post.Title;
                    existingPost.Content = post.Content;
                    existingPost.CreationDate = DateTime.Now;
                    existingPost.Summary = post.Content.Substring(0, Math.Min(post.Content.Length, 250));
                    existingPost.Sentiment = post.Sentiment;

                    _context.Update(existingPost);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    /*
                    TempData["NotificationType"] = "alert-danger";
                    TempData["NotificationMessage"] = "Unable to commit changes to backend database. Please try again.";
                    */
                    // Something prevented the database from committing the changes. Pass the existing post back to the
                    // GET /Blog/Edit view so that the user can easily try again to submit their desired changes
                    return View(existingPost);
                }

                // Success! Redirect to the details for the newly edited post for examination.
                return RedirectToAction("Details", new { id });
            }

            // Either the model was invalid or the post wasn't found. Either way, TODO(taylorjoshuaw): This makes no
            // sense if the existing post wasn't found. Should probably redirect to Index instead. Code freeze means
            // this stays though :-/
            return View(existingPost);
        }

        /// <summary>
        /// GET /Blog/Delete/id?
        /// Try to render the Delete.cshtml view based on the specified id, giving a 404 if the id is blank
        /// or not found within the database.
        /// </summary>
        /// <param name="id">The post id that to prompt for deletion</param>
        /// <returns>ViewResult for the Delete.cshtml view with the specified post as its model on successful
        /// matching with an existing post. On failure, returns a NotFoundResult.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            // No id was provided, return a 404. TODO(taylorjoshuaw): This should be either a redirect or
            //                                                        a BadRequest rather than a 404.
            if (id == null)
            {
                return NotFound();
            }

            // Try to find the first Post entity that matches the specified id
            var post = await _context.Post.SingleOrDefaultAsync(p => p.Id == id);

            // The post was not found. Respond to the client with a 404
            if (post == null)
            {
                return NotFound();
            }

            // Success! Render the delete confirmation view
            return View(post);
        }

        /// <summary>
        /// Commit the deletion of the specified post to the database
        /// </summary>
        /// <param name="id">The id of the post that the user has confirmed needs to
        /// be deleted from the database</param>
        /// <returns>RedirectToActionResult unless an exception is thrown.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Post.SingleOrDefaultAsync(p => p.Id == id);
            _context.Post.Remove(post); // TODO(taylorjoshuaw): No checking if the post exists? :-(
            await _context.SaveChangesAsync(); // TODO(taylorjoshuaw): No exception handling? :-(
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Checks whether a blog post exists. Returns true if it does. False if it doesn't.
        /// </summary>
        /// <param name="id">The id of the post to check for.</param>
        /// <returns>True if the post exists. False if the post does not exist.</returns>
        private bool PostExists(int id)
        {
            // NOTE(taylorjoshuaw): I'm not convinced this method needs to exist.
            return _context.Post.Any(p => p.Id == id);
        }
    }
}
