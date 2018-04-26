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
                ViewResult vr = await controller.Index("cats", null, null) as ViewResult;
                PostIndexViewModel vm = vr.Model as PostIndexViewModel;

                // Assert
                // SingleOrDefault is used to here both ensure that the correct post was
                // selected and that only one post made it past the filter
                Assert.Equal("Bob's First Post", vm.Posts.SingleOrDefault()?.Title);
            }
        }

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
                ViewResult vr = await controller.Index(null, "Bob", null) as ViewResult;
                PostIndexViewModel vm = vr.Model as PostIndexViewModel;

                // Assert
                // SingleOrDefault is used to here both ensure that the correct post was
                // selected and that only one post made it past the filter
                Assert.Equal("Bob's First Post", vm.Posts.SingleOrDefault()?.Title);
            }
        }

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
                ViewResult vr = await controller.Index("cats", "Bob", null) as ViewResult;
                PostIndexViewModel vm = vr.Model as PostIndexViewModel;

                // Assert
                // SingleOrDefault is used to here both ensure that the correct post was
                // selected and that only one post made it past the filter
                Assert.Equal("Bob's First Post", vm.Posts.SingleOrDefault()?.Title);
            }
        }
    }
}