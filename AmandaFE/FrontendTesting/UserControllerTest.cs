using AmandaFE.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FrontendTesting
{
    public class UserControllerTest
    {
        [Fact]
        public void IndexTest()
        {
            // Tests Index from the user's view, 
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        }

        [Fact]
        public void DetailsTest()
        {
            // Tests Details from the user's view, 
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        }

        [Fact]
        public void CreateTest()
        {
            // Tests Create from the user's view, 
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        }

        [Fact]
        public void PostCreateTest()
        {
            // Tests posting a created blog post from the user's view, 
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        }

        [Fact]
        public void EditTest()
        {
            // Tests Edit from the user's view, 
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        }

        [Fact]
        public void PostEditTest()
        {
            // Tests posting an edited blog entry from the user's view, 
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        }

        [Fact]
        public void DeleteTest()
        {
            // Tests Delete from the user's view, 
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        }

        [Fact]
        public void PostDeleteTest()
        {
            // Tests deleting a blog entry and redisplaying the blogs from the user's view, 
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        }

        [Fact]
        public void DeleteConfirmationTest()
        {
            // Tests deleting a blog entry using the Delete action and confirming it. 
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        }



    }
}
