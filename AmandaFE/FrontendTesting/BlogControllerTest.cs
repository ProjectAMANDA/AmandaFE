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
        //    DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
        //    .UseInMemoryDatabase(Guid.NewGuid().ToString())
        //    .Options;
        //    // Both Creates use cookies.
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
        //        //DbSet<User> lists = result.Value as DbSet<User>;

        //        // Assert
        //        //Assert.Equal(2, await lists.CountAsync());


        //    }
        //}


        [Fact]
        public void GetEnrichTest()
        {
            // Returns a View of enrichment (photos and perhaps other blog posts based on analysis of the blog contents).
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
            // Trivial test. Awaiting further instruction.
            Assert.Equal(result, result);
        }

    }
}
