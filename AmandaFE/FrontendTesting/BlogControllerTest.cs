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

        // GET /Blog/Create should return a view for creating new blog posts. With no cookie,
        // the UserName property of the view model should be set to null. Ensure that a ViewResult
        // is returned with the correct view model.
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
    }
}