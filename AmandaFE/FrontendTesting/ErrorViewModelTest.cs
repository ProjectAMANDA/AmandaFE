using AmandaFE.Models;
using System;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions;
using Xunit.Sdk;

namespace FrontendTesting
{
    public class ErrorViewModelTest
    {
        [Fact]
        public void CanGetRequestId()
        {
            // Arrange
            ErrorViewModel vm = new ErrorViewModel()
            {
                RequestId = "Hello, world!"
            };

            // Assert
            Assert.Equal("Hello, world!", vm.RequestId);
        }

        [Fact]
        public void CanSetRequestId()
        {
            // Arrange
            ErrorViewModel vm = new ErrorViewModel()
            {
                RequestId = "Hello, world!"
            };

            // Act
            vm.RequestId = "Goodbye";

            // Assert
            Assert.Equal("Goodbye", vm.RequestId);
        }

        [Fact]
        public void CanGetShowRequestId()
        {
            // Arrange
            ErrorViewModel vm = new ErrorViewModel()
            {
                RequestId = "Hello"
            };

            // Assert
            Assert.True(vm.ShowRequestId);
        }
    }
}
