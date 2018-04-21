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
    public class BlogControllerTest
    {
        [Fact]
        public void GetContextTest()
        {
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
               .UseInMemoryDatabase(Guid.NewGuid().ToString())
               .Options;

            // Arrange

            var context = new BlogDBContext(options);

            // Assert
            Assert.Equal("AmandaFE.Data.BlogDBContext", context.ToString());
        }

        //[Fact]
        //public void IndexTest()
        //{
        // //Index is the search method, for finding posts based on user or keyword.  It takes in the string for the user search and the keyword search, and a parameter called page which can be nullable.  It outputs View tied to the View Model vm.  To test, simulate a few blog entries in a database, then access them.

        //    DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
        //        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        //        .Options;

        //    // Arrange

        //    var context = new BlogDBContext(options);
        //    var controller = new BlogController(context)
        //    {

        //    };

        //    // Act
        //    var result = controller.Index();
        //    var redirectResult = result as BlogController;

        //    // Assert
        //    // 302 is the status code of a RedirectToAction?
        //    Assert.Equal(302, redirectResult.StatusCode);
        //}

        //[Fact]
        //public async void GetCreateTest()
        //{
        //    //Create creates a new blog entry.  It uses Task<IActionResult and uses cookies. It returns View tied to a view model.  To test, simulate getting a blog entry recently put into a database.

        //    DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
        //    .UseInMemoryDatabase(Guid.NewGuid().ToString())
        //    .Options;
        //    // Both Creates use cookies.  Account for them here
        //    // Arrange

        //    //var context = new BlogDBContext(options);
        //    using (BlogDBContext context = new BlogDBContext(options))
        //    {
        //        // Arrange
        //        await context.User.AddRangeAsync(
        //            new User()
        //            {
        //                Name = "Arthur",
        //            },

        //            new User()
        //            {
        //                Name = "Fred",
        //            }
        //        );

        //        await context.SaveChangesAsync();

        //        BlogController controller = new BlogController(context);

        //        // Act
        //        var result = await controller.Create();
        //        var temp = 10;
        //        DbSet<User> lists = result.Value as DbSet<User>;

        //        // Assert
        //        Assert.Equal(2, await lists.CountAsync());

        //    }
        //}

        //[Fact]
        //public async void PostCreateTest()
        //{
        //    //Create creates a new blog entry.  It uses Task<IActionResult and uses cookies. It returns View tied to a view model.  To test, simulate creating a blog entry into a database.

        //    DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
        //    .UseInMemoryDatabase(Guid.NewGuid().ToString())
        //    .Options;

        //}

        [Fact]
        public void GetEnrichTest()
        {
            // Tests Enrich, which returns a View of enrichment (photos and perhaps other blog posts based on analysis of the blog contents).  To test, simulate a database, and return the enriched data.
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

            // Arrange
            var context = new BlogDBContext(options);
            //using (BlogDBContext context = new BlogDBContext(options))
            {

            }
            BlogController controller = new BlogController(context);


            // Act
            var result = controller.Enrich(24);


            // Asset
            // Test in progress. Awaiting further instruction.
            Console.WriteLine($"Result: {result.ToString()}");
            Assert.Equal("Something", result.ToString());
        }

        [Fact]
        public void PostEnrichTest()
        {
            // Tests Enrich, which returns a View of enrichment (photos and perhaps other blog posts based on analysis of the blog contents).  To test, simulate a database, and make a new blog post with the enriched data.
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        }

        [Fact]
        public void DetailsTest()
        {
            // Tests Details, which handles the details in a blog.  To test, ?
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        }

        [Fact]
        public void FindTest()
        {
            // Tests Find, the remnant of the search function now mostly handled by Index.
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        }

        [Fact]
        public void EditTest()
        {
            // Tests Edit, which is used in editing a blog post.  Test by ?
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        }
        [Fact]
        public void PostEditTest()
        {
            // Tests posting to Edit, which post the blog entry after it has been edited.  To test, simulate testing a blog post in a database and reposting it.
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        }

        [Fact]
        public void DeleteTest()
        {
            // Tests Delete, which deletes a blog entry.
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        }

        [Fact]
        public void PostDeleteTest()
        {
            // Tests removing a deleted blog entry.
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        }

    }
}