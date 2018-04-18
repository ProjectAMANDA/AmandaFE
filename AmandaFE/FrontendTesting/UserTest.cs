using AmandaFE.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FrontendTesting
{
    public class UserTest
    {
        [Fact]
        public void GetIdTest()
        {
            // Arrange
            User user = new User
            {
                Id = 42
            };

            // Assert
            Assert.Equal(42, user.Id);
        }

        [Fact]
        public void PutIdTest()
        {
            // Arrange
            User user = new User
            {
                Id = 42
            };

            // Act
            user.Id = 57;

            // Assert
            Assert.Equal(57, user.Id);
        }

        [Fact]
        public void GetNameTest()
        {
            // Arrange
            User user = new User
            {
                Name = "Arthur"
            };

            // Assert
            Assert.Equal("Arthur", user.Name);
        }

        [Fact]
        public void SetNameTest()
        {
            // Arrange
            User user = new User
            {
                Name = "Arthur"
            };

            // Act
            user.Name = "Melvin";

            // Assert
            Assert.Equal("Melvin", user.Name);
        }
    }
}
