using AmandaFE.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FrontendTesting
{
    public class PostTest
    {

        [Fact]
        public void GetIdTest()
        {
            // Arrange
            Post post = new Post()
            {
                Id = 4
            };

            // Assert
            Assert.Equal(4, post.Id);
        }

        [Fact]
        public void SetIdTest()
        {
            // Arrange
            Post post = new Post()
            {
                Id = 4
            };

            // Act
            post.Id = 5;

            // Assert
            Assert.Equal(5, post.Id);
        }

        [Fact]
        public void GetUserIdTest()
        {
            // Arrange
            Post post = new Post()
            {
                Id = 42
            };

            // Assert
            Assert.Equal(42, post.Id);
        }

        [Fact]
        public void SetUserIdTest()
        {
            // Arrange
            Post post = new Post()
            {
                Id = 42
            };

            // Act
            post.Id = 57;

            // Assert
            Assert.Equal(57, post.Id);
        }

        [Fact]
        public void GetUserTest()
        {
            // Arrange
            Post post = new Post()
            {
                User = new User()
                {
                    Name = "Arthur"
                }
            };

            // Assert
            Assert.Equal("Arthur", post.User.Name);

        }

        [Fact]
        public void SetUserTest()
        {
            // Arrange
            Post post = new Post()
            {
                User = new User()
                {
                    Name = "Arthur"
                }
            };

            // Act

            post.User.Name = "Fred";

            // Assert
            Assert.Equal("Fred", post.User.Name);
        }

        [Fact]
        public void GetRelatedPostsTest()
        {
            // Arrange 
            Post post = new Post()
            {
                RelatedPosts = new List<Post>
                {
                    new Post()
                }
            };

            // Assert
            Assert.Single(post.RelatedPosts);
        }

        [Fact]
        public void GetImageHrefTest()
        {
            // Arrange
            Post post = new Post()
            {
                ImageHref = "Test"
            };

            // Assert
            Assert.Equal("Test", post.ImageHref);
        }

        [Fact]
        public void SetImageHrefTest()
        {
            // Arrange
            Post post = new Post()
            {
                ImageHref = "Test"
            };

            // Act
            post.ImageHref = "Foo";

            // Assert
            Assert.Equal("Foo", post.ImageHref);
        }

        [Fact]
        public void SetTitleTest()
        {
            // Arrange
            Post post = new Post()
            {
                Title = "Test"
            };

            // Assert
            Assert.Equal("Test", post.Title);
        }

        [Fact]
        public void GetTitleTest()
        {
            // Arrange
            Post post = new Post()
            {
                Title = "Test"
            };

            // Act
            post.Title = "Bar";

            // Assert
            Assert.Equal("Bar", post.Title);
        }

        [Fact]
        public void GetSummaryTest()
        {
            // Arrange
            Post post = new Post()
            {
                Summary = "Test"
            };

            // Assert
            Assert.Equal("Test", post.Summary);
        }

        [Fact]
        public void SetSummaryTest()
        {
            // Arrange
            Post post = new Post()
            {
                Summary = "Test"
            };

            // Act
            post.Summary = "Fizz";

            // Assert
            Assert.Equal("Fizz", post.Summary);
        }

        [Fact]
        public void GetContentTest()
        {
            // Arrange
            Post post = new Post()
            {
                Content = "Test"
            };

            // Assert
            Assert.Equal("Test", post.Content);
        }

        [Fact]
        public void SetContentTest()
        {
            // Arrange
            Post post = new Post()
            {
                Content = "Test"
            };

            // Act
            post.Content = "Buzz";

            // Assert
            Assert.Equal("Buzz", post.Content);
        }

        [Fact]
        public void GetRelatedArticlesTest()
        {
            // Arrange
            Post post = new Post()
            {
                RelatedArticles = "Test"
            };

            // Assert
            Assert.Equal("Test", post.RelatedArticles);
        }

        [Fact]
        public void SetRelatedArticlesTest()
        {
            // Arrange
            Post post = new Post()
            {
                RelatedArticles = "Test"
            };

            // Act
            post.RelatedArticles = "Chess";

            // Assert
            Assert.Equal("Chess", post.RelatedArticles);
        }

        [Fact]
        public void GetSentimentTest()
        {
            // Arrange
            Post post = new Post()
            {
                Sentiment = 42
            };

            // Assert
            Assert.Equal(42, post.Sentiment);
        }

        [Fact]
        public void SetSentimentTest()
        {
            // Arrange
            Post post = new Post()
            {
                Sentiment = 42
            };

            // Act
            post.Sentiment = 57;

            // Assert
            Assert.Equal(57, post.Sentiment);
        }

        [Fact]
        public void GetKeywordsTest()
        {
            // Arrange
            Post post = new Post()
            {
                Keywords = "Test"
            };

            // Assert
            Assert.Equal("Test", post.Keywords);
        }

        [Fact]
        public void SetKeywordsTest()
        {
            // Arrange
            Post post = new Post()
            {
                Keywords = "Test"
            };

            // Act
            post.Keywords = "Checkers";

            // Assert
            Assert.Equal("Checkers", post.Keywords);
        }


        [Fact]
        public void GetCreationDate()
        {
            // Arrange
            Post post = new Post()
            {
                CreationDate = new DateTime(2018, 4, 18)
            };

            // Assert
            Assert.Equal("April 18, 2018", post.CreationDate.ToString("MMMM dd, yyyy"));
        }

        [Fact]
        public void SetCreationDate()
        {
            // Arrange
            Post post = new Post()
            {
                CreationDate = new DateTime(2018, 4, 18)
            };

            // Act
            post.CreationDate = new DateTime(1967, 11, 22);

            // Assert
            Assert.Equal("November 22, 1967", post.CreationDate.ToString("MMMM dd, yyyy"));


        }
    }  
}
