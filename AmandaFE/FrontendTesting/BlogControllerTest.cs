using AmandaFE;
using AmandaFE.Controllers;
using AmandaFE.Data;
using AmandaFE.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace FrontendTesting
{
    public class BlogControllerTest
    {
        // Tests the Index action and ensures that it gets a proper view model
        // for rendering based on a mocked cookie collection. This test does
        // not provide any parameters to the Index action.
        [Fact]
        public async void CanGetIndexAndViewModelWithoutArguments()
        {
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (BlogDBContext context = new BlogDBContext(options))
            {
                // Arrange
                BlogController controller = new BlogController(context);

                User testUser = new User()
                {
                    Name = "Bob"
                };

                await context.User.AddAsync(testUser);
                await context.SaveChangesAsync();

                Post testPost = new Post()
                {
                    Content = "Hello, world!",
                    CreationDate = DateTime.Today,
                    ImageHref = "http://not.a.real/website.png",
                    Sentiment = 0.625f,
                    Summary = "Hello, world!",
                    Title = "Bob's First Post",
                    UserId = testUser.Id
                };

                await context.Post.AddAsync(testPost);
                await context.SaveChangesAsync();

                // Act
                ViewResult vr = await controller.Index(null, null, null) as ViewResult;
                PostIndexViewModel vm = vr.Model as PostIndexViewModel;

                // Assert
                Assert.Equal("Bob's First Post", vm.Posts.FirstOrDefault()?.Title);
            }
        }

        // GET /Blog/Index?searchKeywordString="cats" should return an Index view
        // with all posts related to the "cats" keyword via the PostKeyword
        // junction table. Ensure that the filtering occurs correctly for the view
        // model.
        [Fact]
        public async void CanGetIndexAndViewModelWithKeywordSearch()
        {
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (BlogDBContext context = new BlogDBContext(options))
            {
                // Arrange
                BlogController controller = new BlogController(context);

                User testUser = new User()
                {
                    Name = "Bob"
                };

                await context.User.AddAsync(testUser);
                await context.SaveChangesAsync();

                Post testPost = new Post()
                {
                    Content = "Hello, world!",
                    CreationDate = DateTime.Today,
                    ImageHref = "http://not.a.real/website.png",
                    Sentiment = 0.625f,
                    Summary = "Hello, world!",
                    Title = "Bob's First Post",
                    UserId = testUser.Id
                };

                Post testPost2 = new Post()
                {
                    Content = "Goodbye, world!",
                    CreationDate = DateTime.Today,
                    ImageHref = "http://not.a.real/website.png",
                    Sentiment = 0.625f,
                    Summary = "Goodbye, world!",
                    Title = "Bob's Second Post",
                    UserId = testUser.Id
                };

                await context.Post.AddAsync(testPost);
                await context.Post.AddAsync(testPost2);
                await context.SaveChangesAsync();

                // Add in some keywords to our post for testing the keyword search
                await KeywordUtilities.MergeKeywordStringIntoPostAsync("cats, dogs", testPost.Id, context);

                // Act
                ViewResult vr = await controller.Index("cats", null, null) as ViewResult;
                PostIndexViewModel vm = vr.Model as PostIndexViewModel;

                // Assert
                // SingleOrDefault is used to here both ensure that the correct post was
                // selected and that only one post made it past the filter
                Assert.Equal("Bob's First Post", vm.Posts.SingleOrDefault()?.Title);
            }
        }

        // GET /Blog/Index?searchUserName=Bob should return an index view with the
        // view model's Posts property filtered to only include posts created by the
        // users named "Bob". Ensure that the filtering behavior works correctly and
        // that a proper ViewResult is returned.
        [Fact]
        public async void CanGetIndexAndViewModelWithUserSearch()
        {
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (BlogDBContext context = new BlogDBContext(options))
            {
                // Arrange
                BlogController controller = new BlogController(context);

                User testUser = new User()
                {
                    Name = "Bob"
                };

                User testUser2 = new User()
                {
                    Name = "Doug"
                };

                await context.User.AddAsync(testUser);
                await context.User.AddAsync(testUser2);
                await context.SaveChangesAsync();

                Post testPost = new Post()
                {
                    Content = "Hello, world!",
                    CreationDate = DateTime.Today,
                    ImageHref = "http://not.a.real/website.png",
                    Sentiment = 0.625f,
                    Summary = "Hello, world!",
                    Title = "Bob's First Post",
                    UserId = testUser.Id
                };

                Post testPost2 = new Post()
                {
                    Content = "Goodbye, world!",
                    CreationDate = DateTime.Today,
                    ImageHref = "http://not.a.real/website.png",
                    Sentiment = 0.625f,
                    Summary = "Goodbye, world!",
                    Title = "Doug's First Post",
                    UserId = testUser2.Id
                };

                await context.Post.AddAsync(testPost);
                await context.Post.AddAsync(testPost2);
                await context.SaveChangesAsync();

                // Act
                ViewResult vr = await controller.Index(null, "Bob", null) as ViewResult;
                PostIndexViewModel vm = vr.Model as PostIndexViewModel;

                // Assert
                // SingleOrDefault is used to here both ensure that the correct post was
                // selected and that only one post made it past the filter
                Assert.Equal("Bob's First Post", vm.Posts.SingleOrDefault()?.Title);
            }
        }

        // GET /Blog/Index?searchKeywordString=cats&searchUserName=Bob should return a view
        // with a view model only containing posts that match the filtering criteria of
        // being created by users named "Bob" AND which have relationships to the "cats" keyword
        // via the PostKeyword junction table.
        [Fact]
        public async void CanGetIndexAndViewModelWithKeywordAndUserSearch()
        {
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (BlogDBContext context = new BlogDBContext(options))
            {
                // Arrange
                BlogController controller = new BlogController(context);

                User testUser = new User()
                {
                    Name = "Bob"
                };

                User testUser2 = new User()
                {
                    Name = "Doug"
                };

                await context.User.AddAsync(testUser);
                await context.User.AddAsync(testUser2);
                await context.SaveChangesAsync();

                // Create two posts to ensure that the filtering is
                // actually selecting only matching posts
                Post testPost = new Post()
                {
                    Content = "Hello, world!",
                    CreationDate = DateTime.Today,
                    ImageHref = "http://not.a.real/website.png",
                    Sentiment = 0.625f,
                    Summary = "Hello, world!",
                    Title = "Bob's First Post",
                    UserId = testUser.Id
                };

                Post testPost2 = new Post()
                {
                    Content = "Goodbye, world!",
                    CreationDate = DateTime.Today,
                    ImageHref = "http://not.a.real/website.png",
                    Sentiment = 0.625f,
                    Summary = "Goodbye, world!",
                    Title = "Doug's First Post",
                    UserId = testUser2.Id
                };

                await context.Post.AddAsync(testPost);
                await context.Post.AddAsync(testPost2);
                await context.SaveChangesAsync();

                // Add in some keywords to our post for testing the keyword search
                await KeywordUtilities.MergeKeywordStringIntoPostAsync("cats, dogs", testPost.Id, context);

                // Act
                ViewResult vr = await controller.Index("cats", "Bob", null) as ViewResult;
                PostIndexViewModel vm = vr.Model as PostIndexViewModel;

                // Assert
                // SingleOrDefault is used to here both ensure that the correct post was
                // selected and that only one post made it past the filter
                Assert.Equal("Bob's First Post", vm.Posts.SingleOrDefault()?.Title);
            }
        }

        // GET /Blog/Create should return a view for creating new blog posts. With no cookie,
        // the UserName property of the view model should be set to null. Ensure that a ViewResult
        // is returned with the correct view model.
        [Fact]
        public async void CanGetCreateViewWithoutCookie()
        {
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (BlogDBContext context = new BlogDBContext(options))
            {
                // Arrange
                BlogController controller = new BlogController(context);

                // MOQ for HttpRequest
                // Even though we are testing the functionality of this action without
                // a user cookie, we still need a properly mocked HttpContext to check
                // for the presence of keys from the called Cookies.GetUserFromCookieAsync
                // method. We will just make sure ContainsKey returns false to trigger the
                // behavior that would be called if no user cookie has been stored in the
                // user's browser.
                var mockCookieCollection = new Mock<IRequestCookieCollection>(MockBehavior.Strict);
                mockCookieCollection.Setup(cc => cc.ContainsKey(Cookies.UserId))
                    .Returns(false);

                var mockRequest = new Mock<HttpRequest>(MockBehavior.Strict);
                mockRequest.SetupGet(r => r.Cookies)
                    .Returns(mockCookieCollection.Object);

                var mockContext = new Mock<HttpContext>();
                mockContext.SetupGet(c => c.Request)
                    .Returns(mockRequest.Object);

                // Overwrite the controller's context with our mocked objects to provide
                // access to cookies in the test
                controller.ControllerContext = new ControllerContext(new ActionContext(
                    mockContext.Object, new RouteData(),
                    new ControllerActionDescriptor()));

                // Act
                ViewResult vr = await controller.Create() as ViewResult;

                // Assert
                PostCreateViewModel vrViewModel = vr.Model as PostCreateViewModel;
                Assert.Null(vrViewModel.UserName);
            }
        }

        // GET /Blog/Create should return a view for creating new blog posts. With a cookie
        // present in the user's browser, the user's name should be filled into the view
        // model from the database. Ensure that the user is identified via their cookie
        // and filled into the view model.
        [Fact]
        public async void CanGetCreateViewWithCookie()
        {
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (BlogDBContext context = new BlogDBContext(options))
            {
                // Arrange
                BlogController controller = new BlogController(context);

                User testUser = new User()
                {
                    Name = "Bob"
                };

                await context.User.AddAsync(testUser);
                await context.SaveChangesAsync();

                // MOQ for HttpRequest (needed for cookies)
                var mockCookieCollection = new Mock<IRequestCookieCollection>(MockBehavior.Strict);
                mockCookieCollection.Setup(cc => cc.ContainsKey(Cookies.UserId))
                    .Returns(true);
                mockCookieCollection.Setup(cc => cc[Cookies.UserId])
                    .Returns(testUser.Id.ToString());

                var mockRequest = new Mock<HttpRequest>(MockBehavior.Strict);
                mockRequest.SetupGet(r => r.Cookies)
                    .Returns(mockCookieCollection.Object);

                var mockContext = new Mock<HttpContext>();
                mockContext.SetupGet(c => c.Request)
                    .Returns(mockRequest.Object);

                // Overwrite the controller's context with our mocked objects to provide
                // access to cookies in the test
                controller.ControllerContext = new ControllerContext(new ActionContext(
                    mockContext.Object, new RouteData(),
                    new ControllerActionDescriptor()));

                // Act
                ViewResult vr = await controller.Create() as ViewResult;

                // Assert
                PostCreateViewModel vrViewModel = vr.Model as PostCreateViewModel;
                Assert.Equal("Bob", vrViewModel.UserName);
            }
        }

        // POST /Blog/Create with a bound PostCreateViewModel object should create a new Post entity
        // in the database. Ensure that a new Post entity is created by this action.
        [Fact]
        public async void CanPostCreateAndAddPost()
        {
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (BlogDBContext context = new BlogDBContext(options))
            {
                // Arrange
                BlogController controller = new BlogController(context);

                User testUser = new User()
                {
                    Name = "Bob"
                };

                await context.User.AddAsync(testUser);
                await context.SaveChangesAsync();

                // MOQ for HttpResponse (needed for cookies)
                // Use MockBehavior.Loose since we are not going to actually do anything
                // with the written cookie in this test.
                var mockCookieCollection = new Mock<IResponseCookies>(MockBehavior.Loose);

                var mockResponse = new Mock<HttpResponse>(MockBehavior.Strict);
                mockResponse.SetupGet(r => r.Cookies)
                    .Returns(mockCookieCollection.Object);

                var mockContext = new Mock<HttpContext>();
                mockContext.SetupGet(c => c.Response)
                    .Returns(mockResponse.Object);

                // Overwrite the controller's context with our mocked objects to provide
                // access to cookies in the test
                controller.ControllerContext = new ControllerContext(new ActionContext(
                    mockContext.Object, new RouteData(),
                    new ControllerActionDescriptor()));

                // Compared to the database's posts count after calling Create in the assertion
                int postCountBeforeCreate = await context.Post.CountAsync();

                // Act
                RedirectToActionResult ra = await controller.Create(new PostCreateViewModel()
                {
                    UserName = testUser.Name,
                    PostContent = "Hello, world!",
                    EnrichPost = false,
                    PostTitle = "Bob's First Post",
                    Keywords = "cats, dogs"
                }) as RedirectToActionResult;

                // Assert
                Assert.Equal(postCountBeforeCreate + 1, await context.Post.CountAsync());
            }
        }

        // POST /Blog/Create with a bound PostCreateViewModel object with EnrichPost
        // set to false should be redirected to the Details action and not to the
        // Enrich action. Ensure that Create redirects to the correct action.
        [Fact]
        public async void CanPostCreatePostWithoutEnrichment()
        {
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (BlogDBContext context = new BlogDBContext(options))
            {
                // Arrange
                BlogController controller = new BlogController(context);

                User testUser = new User()
                {
                    Name = "Bob"
                };

                await context.User.AddAsync(testUser);
                await context.SaveChangesAsync();

                // MOQ for HttpResponse (needed for cookies)
                // Use MockBehavior.Loose since we are not going to actually do anything
                // with the written cookie in this test.
                var mockCookieCollection = new Mock<IResponseCookies>(MockBehavior.Loose);

                var mockResponse = new Mock<HttpResponse>(MockBehavior.Strict);
                mockResponse.SetupGet(r => r.Cookies)
                    .Returns(mockCookieCollection.Object);

                var mockContext = new Mock<HttpContext>();
                mockContext.SetupGet(c => c.Response)
                    .Returns(mockResponse.Object);

                // Overwrite the controller's context with our mocked objects to provide
                // access to cookies in the test
                controller.ControllerContext = new ControllerContext(new ActionContext(
                    mockContext.Object, new RouteData(),
                    new ControllerActionDescriptor()));

                // Act
                RedirectToActionResult ra = await controller.Create(new PostCreateViewModel()
                {
                    UserName = testUser.Name,
                    PostContent = "Hello, world!",
                    EnrichPost = false,
                    PostTitle = "Bob's First Post",
                    Keywords = "cats, dogs"
                }) as RedirectToActionResult;

                // Assert
                Assert.Equal("Details", ra.ActionName);
            }
        }

        // POST /Blog/Create with a bound PostCreateViewModel object with EnrichPost
        // set to true should be redirected to the Enrich action and not to the
        // Details action. Ensure that Create redirects to the correct action.
        [Fact]
        public async void CanPostCreatePostWithEnrichment()
        {
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (BlogDBContext context = new BlogDBContext(options))
            {
                // Arrange
                BlogController controller = new BlogController(context);

                User testUser = new User()
                {
                    Name = "Bob"
                };

                await context.User.AddAsync(testUser);
                await context.SaveChangesAsync();

                // MOQ for HttpResponse (needed for cookies)
                // Use MockBehavior.Loose since we are not going to actually do anything
                // with the written cookie in this test.
                var mockCookieCollection = new Mock<IResponseCookies>(MockBehavior.Loose);

                var mockResponse = new Mock<HttpResponse>(MockBehavior.Strict);
                mockResponse.SetupGet(r => r.Cookies)
                    .Returns(mockCookieCollection.Object);

                var mockContext = new Mock<HttpContext>();
                mockContext.SetupGet(c => c.Response)
                    .Returns(mockResponse.Object);

                // Overwrite the controller's context with our mocked objects to provide
                // access to cookies in the test
                controller.ControllerContext = new ControllerContext(new ActionContext(
                    mockContext.Object, new RouteData(),
                    new ControllerActionDescriptor()));

                // Act
                RedirectToActionResult ra = await controller.Create(new PostCreateViewModel()
                {
                    UserName = testUser.Name,
                    PostContent = "Hello, world!",
                    EnrichPost = true,
                    PostTitle = "Bob's First Post",
                    Keywords = "cats, dogs"
                }) as RedirectToActionResult;

                // Assert
                Assert.Equal("Enrich", ra.ActionName);
            }
        }

        // GET /Blog/Enrich/{id?} with a valid Post entity id should present the Enrich view
        // for the specified entity. Ensure that a ViewResult is returned from this action
        // when a valid id is provided.
        [Fact]
        public async void CanGetEnrichView()
        {
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (BlogDBContext context = new BlogDBContext(options))
            {
                // Arrange
                BlogController controller = new BlogController(context);

                User testUser = new User()
                {
                    Name = "Bob"
                };

                await context.User.AddAsync(testUser);
                await context.SaveChangesAsync();

                Post testPost = new Post()
                {
                    Content = "Hello, world!",
                    CreationDate = DateTime.Today,
                    ImageHref = "http://not.a.real/website.png",
                    Sentiment = 0.625f,
                    Summary = "Hello, world!",
                    Title = "Bob's First Post",
                    UserId = testUser.Id
                };

                await context.Post.AddAsync(testPost);
                await context.SaveChangesAsync();

                // Add in keywords to pass to the ProjectAMANDA REST API in Enrich
                await KeywordUtilities.MergeKeywordStringIntoPostAsync("cats, dogs", testPost.Id, context);

                // Act
                IActionResult ar = await controller.Enrich(testPost.Id);

                // Assert
                Assert.IsType<ViewResult>(ar);
            }
        }

        // GET /Blog/Enrich/{id?} with a valid Post entity id should present the Enrich view
        // for the specified entity. Ensure that a ViewResult is returned from this action
        // when a valid id is provided and that its view model contains image results from
        // third party API's via the ProjectAMANDA REST API
        [Fact]
        public async void CanGetEnrichViewWithViewModelContainingImages()
        {
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (BlogDBContext context = new BlogDBContext(options))
            {
                // Arrange
                BlogController controller = new BlogController(context);

                User testUser = new User()
                {
                    Name = "Bob"
                };

                await context.User.AddAsync(testUser);
                await context.SaveChangesAsync();

                Post testPost = new Post()
                {
                    Content = "Hello, world!",
                    CreationDate = DateTime.Today,
                    ImageHref = "http://not.a.real/website.png",
                    Sentiment = 0.625f,
                    Summary = "Hello, world!",
                    Title = "Bob's First Post",
                    UserId = testUser.Id
                };

                await context.Post.AddAsync(testPost);
                await context.SaveChangesAsync();

                // Add in keywords to pass to the ProjectAMANDA REST API in Enrich
                await KeywordUtilities.MergeKeywordStringIntoPostAsync("cats, dogs", testPost.Id, context);

                // Act
                ViewResult vr = await controller.Enrich(testPost.Id) as ViewResult;

                // Assert
                PostEnrichViewModel vrViewModel = vr.Model as PostEnrichViewModel;
                Assert.NotEmpty(vrViewModel.Images);
            }
        }

        // GET /Blog/Enrich/{id?} with a valid Post entity id should present the Enrich view
        // for the specified entity. Ensure that a ViewResult is returned from this action
        // when a valid id is provided and that its view model contains sentiment from Azure
        // Cognitve Services (scores 0.0 - 1.0 indicate Azure enrichment; -1.0 indicates no
        // Azure enrichment)
        [Fact]
        public async void CanGetEnrichViewWithViewModelContainingSentiment()
        {
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (BlogDBContext context = new BlogDBContext(options))
            {
                // Arrange
                BlogController controller = new BlogController(context);

                User testUser = new User()
                {
                    Name = "Bob"
                };

                await context.User.AddAsync(testUser);
                await context.SaveChangesAsync();

                Post testPost = new Post()
                {
                    Content = "Hello, world!",
                    CreationDate = DateTime.Today,
                    ImageHref = "http://not.a.real/website.png",
                    Sentiment = 0.625f,
                    Summary = "Hello, world!",
                    Title = "Bob's First Post",
                    UserId = testUser.Id
                };

                await context.Post.AddAsync(testPost);
                await context.SaveChangesAsync();

                // Add in keywords to pass to the ProjectAMANDA REST API in Enrich
                await KeywordUtilities.MergeKeywordStringIntoPostAsync("cats, dogs", testPost.Id, context);

                // Act
                ViewResult vr = await controller.Enrich(testPost.Id) as ViewResult;

                // Assert
                PostEnrichViewModel vrViewModel = vr.Model as PostEnrichViewModel;

                // Sentiment values of 0.0 to 1.0 indicate enrichment from Azure
                // Cognitive Services. Ranges take into account 32-bit IEEE 754
                // epsilons to protect against floating point errors.
                Assert.True(vrViewModel.Sentiment >= 0.0f - float.Epsilon &&
                    vrViewModel.Sentiment <= 1.0f + float.Epsilon);
            }
        }

        // POST /Blog/Enrich with a bound PostEnrichViewModel object should change the
        // ImageHref property of the specified post. Ensure that this property is
        // modified after calling POST /Blog/Enrich
        [Fact]
        public async void CanPostEnrichAndChangePostImageHref()
        {
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (BlogDBContext context = new BlogDBContext(options))
            {
                // Arrange
                BlogController controller = new BlogController(context);

                User testUser = new User()
                {
                    Name = "Bob"
                };

                await context.User.AddAsync(testUser);
                await context.SaveChangesAsync();

                Post testPost = new Post()
                {
                    Content = "Hello, world!",
                    CreationDate = DateTime.Today,
                    ImageHref = "http://not.a.real/website.png",
                    Sentiment = 0.625f,
                    Summary = "Hello, world!",
                    Title = "Bob's First Post",
                    UserId = testUser.Id
                };

                await context.Post.AddAsync(testPost);
                await context.SaveChangesAsync();

                // Act
                // Simulate the user selecting an image from the Enrich view and
                // enrich the specified post (corresponding to testPost)
                await controller.Enrich(new PostEnrichViewModel
                {
                    PostId = testPost.Id,
                    SelectedImageHref = "http://also.not.a/real/website.png"
                });

                // Assert
                Assert.Equal("http://also.not.a/real/website.png", (await context.Post.FirstAsync()).ImageHref);
            }
        }
    }
}