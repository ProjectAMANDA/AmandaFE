using AmandaFE.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FrontendTesting
{
    public class HomeControllerTest
    {
        [Fact]
        public void IndexTest()
        {
            // Tests Index, the main display page.
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        }

        [Fact]
        public void ErrorTest()
        {
            // Tests Error, what happens when there is an error, such as a blog post that isn't there.
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        }
    }
}
