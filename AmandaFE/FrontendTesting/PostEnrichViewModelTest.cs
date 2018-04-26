using AmandaFE.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FrontendTesting
{
    public class PostEnrichViewModelTest
    {
        [Fact]
        public void GetPostTest()
        {
            // Arrange
            PostEnrichViewModel vm = new PostEnrichViewModel
            {
                Post = new Post
                {
                    Title = "The Title"
                }
            };

            // Assert
            Assert.Equal("The Title", vm.Post.Title);
        }

        [Fact]
        public void SetPostTest()
        {
            // Arrange
            PostEnrichViewModel vm = new PostEnrichViewModel
            {
                Post = new Post
                {
                    Title = "The Title"
                }
            };

            // Act
            vm.Post.Title = "Some Other Title";

            // Assert
            Assert.Equal("Some Other Title", vm.Post.Title);
        }

        [Fact]
        public void GetPostIdTest()
        {
            // Arrange
            PostEnrichViewModel vm = new PostEnrichViewModel
            {
                PostId = 42
            };

            // Assert
            Assert.Equal(42, vm.PostId);
        }

        [Fact]
        public void SetPostIdTest()
        {
            // Arrange
            PostEnrichViewModel vm = new PostEnrichViewModel
            {
                PostId = 42
            };

            // Act
            vm.PostId = 57;

            // Assert
            Assert.Equal(57, vm.PostId);
        }

        /*
        [Fact]
        public void CanGetImageHrefsTest()
        {
            // Arrange
            PostEnrichViewModel vm = new PostEnrichViewModel()
            {
                ImageHrefs = new List<string> { "https://www.google.com/images/branding/googlelogo/2x/googlelogo_color_272x92dp.png" }
            };

            // Assert
            Assert.Single(vm.ImageHrefs);
        }
        */

        [Fact]
        public void GetSelectedImageHref()
        {
            // Arrange
            PostEnrichViewModel vm = new PostEnrichViewModel
            {
                SelectedImageHref = "https://www.google.com/images/branding/googlelogo/2x/googlelogo_color_272x92dp.png"
            };

            // Assert
            Assert.Equal("https://www.google.com/images/branding/googlelogo/2x/googlelogo_color_272x92dp.png", vm.SelectedImageHref);
        }

        [Fact]
        public void SetSelectedImageHref()
        {
            // Arrange
            PostEnrichViewModel vm = new PostEnrichViewModel
            {
                SelectedImageHref = "https://www.google.com/images/branding/googlelogo/2x/googlelogo_color_272x92dp.png"
            };

            // Act
            vm.SelectedImageHref = "https://img-prod-cms-rt-microsoft-com.akamaized.net/cms/api/am/imageFileData/RE1Mu3b?ver=5c31";

            // Assert
            Assert.Equal("https://img-prod-cms-rt-microsoft-com.akamaized.net/cms/api/am/imageFileData/RE1Mu3b?ver=5c31", vm.SelectedImageHref);
        }


    }
}
