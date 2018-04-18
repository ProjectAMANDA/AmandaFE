using AmandaFE.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;

namespace FrontendTesting
{
    public class HomeViewModelTest
    {
        [Fact]
        public void CanGetLastTen()
        {
            // Arrange
            HomeViewModel vm = new HomeViewModel
            {
                LastTen = new List<Post>
                {
                    new Post()
                }
            };

            // Assert
            Assert.Single(vm.LastTen);
        }
    }
}
