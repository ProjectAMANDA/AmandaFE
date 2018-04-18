using AmandaFE.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FrontendTesting
{
    public class UserViewModelTest
    {
        [Fact]
        public void GetUserTest()
        {
            // Arrange
            UserViewModel vm = new UserViewModel
            {
                User = new User
                {
                    Name = "Arthur"
                }
            };

            // Assert
            Assert.Equal("Arthur", vm.User.Name);
        }

        [Fact]
        public void SetUserTest()
        {
            // Arrange
            UserViewModel vm = new UserViewModel
            {
                User = new User
                {
                    Name = "Arthur"
                }
            };

            // Act
            vm.User.Name = "Tiger";

            // Assert
            Assert.Equal("Tiger", vm.User.Name);
        }

        [Fact]
        public void CanGetLastFiveTest()
        {
            // Arrange
            UserViewModel vm = new UserViewModel()
            {
                LastFive = new List<Post>
                {
                    new Post()
                }
            };

            // Assert
            Assert.Single(vm.LastFive);
        }
    }
}
