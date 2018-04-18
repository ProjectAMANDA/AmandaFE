using Xunit;
using System;
using System.Collections.Generic;
using System.Text;
using AmandaFE.Models;

namespace FrontendTesting
{
    public class PostCreateViewModelTest
    {
        [Fact]
        public void GetUserNameTest()
        {
            // Arrange
            PostCreateViewModel vm = new PostCreateViewModel
            {
                UserName = "Arthur"
            };

            // Assert
            Assert.Equal("Arthur", vm.UserName);
        }

        [Fact]
        public void SetUserNameTest()
        {
            // Arrange
            PostCreateViewModel vm = new PostCreateViewModel
            {
                UserName = "Arthur"
            };

            // Act
            vm.UserName = "Fred";

            // Assert
            Assert.Equal("Fred", vm.UserName);
        }

        [Fact]
        public void GetPostTitleTest()
        {
            // Arrange
            PostCreateViewModel vm = new PostCreateViewModel
            {
                PostTitle = "Title"
            };

            // Assert
            Assert.Equal("Title", vm.PostTitle);
        }

        [Fact]
        public void SetPostTitleTest()
        {
            // Arrange
            PostCreateViewModel vm = new PostCreateViewModel
            {
                PostTitle = "Title"
            };

            // Act
            vm.PostTitle = "Gone With the Wind";

            // Assert
            Assert.Equal("Gone With the Wind", vm.PostTitle);
        }

        [Fact]
        public void GetPostContentTest()
        {
            // Arrange
            PostCreateViewModel vm = new PostCreateViewModel
            {
                PostContent = "Blah, blah, blah."
            };

            // Assert
            Assert.Equal("Blah, blah, blah.", vm.PostContent);
        }

        [Fact]
        public void SetPostContentTest()
        {
            // Arrange
            PostCreateViewModel vm = new PostCreateViewModel
            {
                PostContent = "Blah, blah, blah."
            };

            // Act
            vm.PostContent = "Some insightful content.";

            // Assert
            Assert.Equal("Some insightful content.", vm.PostContent);
        }

        [Fact]
        public void CanGetEnrichPostTest()
        {
            // Arrange
            PostCreateViewModel vm = new PostCreateViewModel
            {
                EnrichPost = true
            };

            // Assert
            Assert.True(vm.EnrichPost);
        }
    }
}
