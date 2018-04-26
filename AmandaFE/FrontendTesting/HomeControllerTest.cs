using AmandaFE.Controllers;
using AmandaFE.Data;
using AmandaFE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FrontendTesting
{
    public class HomeControllerTest
    {
        // GET /Home/Index should return a ViewResult. Ensure that the return type
        // is a ViewResult.
        [Fact]
        public async void CanGetIndexView()
        {
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (BlogDBContext context = new BlogDBContext(options))
            {
                // Arrange
                HomeController controller = new HomeController(context);

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
                IActionResult ar = await controller.Index();

                // Assert
                Assert.IsType<ViewResult>(ar);
            }
        }

        // GET /Home/Index should return a ViewResult with all posts in the database
        // in its view model. Ensure that the view model is populated with the
        // database's post.
        [Fact]
        public async void CanGetIndexViewWithAllPosts()
        {
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (BlogDBContext context = new BlogDBContext(options))
            {
                // Arrange
                HomeController controller = new HomeController(context);

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
                ViewResult vr = await controller.Index() as ViewResult;

                // Assert
                PostIndexViewModel vrViewModel = vr.Model as PostIndexViewModel;
                Assert.Single(vrViewModel.Posts);
            }
        }

        // GET /Home/Error should return the Error view. Ensure that this is the
        // returned type.
        /* TODO(taylorjoshuaw): Figure out how to make this test pass
        [Fact]
        public void CanGetErrorView()
        {
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (BlogDBContext context = new BlogDBContext(options))
            {
                // Arrange
                HomeController controller = new HomeController(context);

                // Assert
                Assert.IsType<ViewResult>(controller.Error());
            }
        }
        */
    }
}
