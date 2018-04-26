using AmandaFE.Controllers;
using AmandaFE.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using AmandaFE;
using AmandaFE.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace FrontendTesting
{
    public class UserControllerTest
    {
        // Tests the index action and ensures that it gets a proper view model
        // for rendering based on a mocked cookie collection
        [Fact]
        public async void CanGetIndexWithViewModel()
        {
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (BlogDBContext context = new BlogDBContext(options))
            {
                // Arrange
                UserController controller = new UserController(context);

                User testUser = new User()
                {
                    Name = "Bob"
                };

                await context.User.AddAsync(testUser);
                await context.SaveChangesAsync();

                Post testPost = new Post()
                {
                    Content = "Hello, world!",
                    CreationDate = DateTime.Today,
                    ImageHref = "http://not.a.real/website.png",
                    Sentiment = 0.625f,
                    Summary = "Hello, world!",
                    Title = "Bob's First Post",
                    UserId = testUser.Id
                };

                await context.Post.AddAsync(testPost);
                await context.SaveChangesAsync();

                // MOQ for HttpRequest (needed for cookies)
                var mockCookieCollection = new Mock<IRequestCookieCollection>(MockBehavior.Strict);
                mockCookieCollection.Setup(cc => cc.ContainsKey(Cookies.UserId))
                    .Returns(true);
                mockCookieCollection.Setup(cc => cc[Cookies.UserId])
                    .Returns(testUser.Id.ToString());

                var mockRequest = new Mock<HttpRequest>(MockBehavior.Strict);
                mockRequest.SetupGet(r => r.Cookies)
                    .Returns(mockCookieCollection.Object);

                var mockContext = new Mock<HttpContext>();
                mockContext.SetupGet(c => c.Request)
                    .Returns(mockRequest.Object);

                // Overwrite the controller's context with our mocked objects to provide
                // access to cookies in the test
                controller.ControllerContext = new ControllerContext(new ActionContext(
                    mockContext.Object, new RouteData(),
                    new ControllerActionDescriptor()));

                // Act
                ViewResult vr = await controller.Index() as ViewResult;
                UserViewModel vm = vr.Model as UserViewModel;

                // Assert
                Assert.Equal("Bob's First Post", vm.AllPosts.FirstOrDefault().Title);
            }
        }

        // GET /User/Details should redirect to the /Blog/Index action with route data
        // of the user name corresponding to the id parameter in the database
        [Fact]
        public async void CanGetDetailsRedirectToBlogIndex()
        {
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (BlogDBContext context = new BlogDBContext(options))
            {
                // Arrange
                UserController controller = new UserController(context);

                User testUser = new User()
                {
                    Name = "Bob"
                };

                await context.User.AddAsync(testUser);
                await context.SaveChangesAsync();

                Post testPost = new Post()
                {
                    Content = "Hello, world!",
                    CreationDate = DateTime.Today,
                    ImageHref = "http://not.a.real/website.png",
                    Sentiment = 0.625f,
                    Summary = "Hello, world!",
                    Title = "Bob's First Post",
                    UserId = testUser.Id
                };

                await context.Post.AddAsync(testPost);
                await context.SaveChangesAsync();

                // Act
                RedirectToActionResult vr = await controller.Details(testUser.Id) as RedirectToActionResult;

                // Assert
                Assert.Equal("Bob", vr.RouteValues["searchUserName"]);
            }
        }

        // GET /User/Create should simply return a ViewResult for the user Create view.
        // Verify that the result of this action is indeed a ViewResult.
        [Fact]
        public void CanGetCreateView()
        {
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (BlogDBContext context = new BlogDBContext(options))
            {
                // Arrange
                UserController controller = new UserController(context);

                // Act
                IActionResult ar = controller.Create();

                // Assert
                Assert.IsType<ViewResult>(ar);
            }
        }

        // POST /User/Create should create a new user based on the bound data from the
        // Create view. Check the state of an in-memory database before calling Create
        // and compare it to the state following the call to Create to ensure that a new
        // user has indeed been created and committed.
        [Fact]
        public async void CanPostCreateAndAddUser()
        {
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (BlogDBContext context = new BlogDBContext(options))
            {
                // Arrange
                UserController controller = new UserController(context);
                int userCountBeforeCreate = await context.User.CountAsync();

                // Act
                await controller.Create(new User()
                {
                    Name = "Bob"
                });

                // Assert
                Assert.Equal(userCountBeforeCreate + 1, await context.User.CountAsync());
            }
        }

        // GET /User/Edit/{id?} should return a ViewResult with a User object for the model
        // corresponding to the User at the provided id. Test that the returned view contains
        // the correct, actual user from the database.
        [Fact]
        public async void CanGetEditViewForExistingUser()
        {
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (BlogDBContext context = new BlogDBContext(options))
            {
                // Arrange
                UserController controller = new UserController(context);

                User testUser = new User()
                {
                    Name = "Bob"
                };

                await context.User.AddAsync(testUser);
                await context.SaveChangesAsync();

                // Act
                ViewResult vr = await controller.Edit(testUser.Id) as ViewResult;

                // Assert
                User editModel = vr.Model as User;
                Assert.Equal("Bob", editModel.Name);
            }
        }

        // GET /User/Edit/{id?} should return a 404 if no id is provided. Ensure that
        // the action result is indeed a NotFoundResult and not a ViewResult if no id
        // is specified.
        [Fact]
        public async void CannotGetEditViewWithoutId()
        {
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (BlogDBContext context = new BlogDBContext(options))
            {
                // Arrange
                UserController controller = new UserController(context);

                // Even though we are purposely setting the id to null below, this test is
                // not whether Edit can handle an empty database, it is to test the behavior
                // of a null id input.
                User testUser = new User()
                {
                    Name = "Bob"
                };

                await context.User.AddAsync(testUser);
                await context.SaveChangesAsync();

                // Act
                IActionResult ar = await controller.Edit(null);

                // Assert
                Assert.IsType<NotFoundResult>(ar);
            }
        }

        // POST /User/Edit/{id} with a bound User object from the Edit view's form data should
        // modify an existing entity corresponding to the provided id on the database.
        // Ensure that the entity on the database is actually modified by the HttpPost
        // Edit action.

        // TODO(taylorjoshuaw): Add test cases for POST /User/Edit with an invalid id and for
        //                      a bound User object with an invalid ModelState
        [Fact]
        public async void CanPostEditAndModifyExistingUserEntity()
        {
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (BlogDBContext context = new BlogDBContext(options))
            {
                // Arrange
                UserController controller = new UserController(context);

                User testUser = new User()
                {
                    Name = "Bob"
                };

                await context.User.AddAsync(testUser);
                await context.SaveChangesAsync();

                // This is not the best way to test this, but Entity Framework does not like
                // it when we instantiate another instance of User with the same Id as testUser
                testUser.Name = "Doug";

                // Act
                await controller.Edit(testUser.Id, testUser);

                // Assert
                // Query for the same entity as testUser from the database to ensure that changes
                // were properly committed to the database via POST /User/Edit?id=testUser.Id
                User modifiedTestUser = await context.User.FindAsync(testUser.Id);
                Assert.Equal("Doug", modifiedTestUser.Name);
            }
        }


        // GET /User/Delete/{id?} should present a confirmation view to the user when a valid
        // id has been provided. Ensure that his happens and that the correct User model is
        // passed along to the returned ViewResult.

        // TODO(taylorjoshuaw): Write test cases for GET /User/Delete with an invalid user id,
        //                      and for a null id.
        [Fact]
        public async void CanGetDeleteViewWithUserModel()
        {
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (BlogDBContext context = new BlogDBContext(options))
            {
                // Arrange
                UserController controller = new UserController(context);

                User testUser = new User()
                {
                    Name = "Bob"
                };

                await context.User.AddAsync(testUser);
                await context.SaveChangesAsync();

                // Act
                ViewResult vr = await controller.Delete(testUser.Id) as ViewResult;

                // Assert
                User vrModel = vr.Model as User;
                Assert.Equal("Bob", vrModel.Name);
            }
        }

        // DELETE /User/Delete/{id} should delete the entity corresponding to the provided id
        // and commit the deletion to the database. Ensure that this removal actually occurs.
        [Fact]
        public async void PostDeleteTest()
        {
            DbContextOptions<BlogDBContext> options = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (BlogDBContext context = new BlogDBContext(options))
            {
                // Arrange
                UserController controller = new UserController(context);

                User testUser = new User()
                {
                    Name = "Bob"
                };

                await context.User.AddAsync(testUser);
                await context.SaveChangesAsync();

                // Act
                // Store the user count in the database before DELETE /User/Delete?id=testUser.Id
                // for comparison in the assertion below.
                int userCountBeforeDelete = await context.User.CountAsync();
                await controller.DeleteConfirmation(testUser.Id);

                // Assert
                // There should be one fewer user than the user count before DELETE /User/Delete
                Assert.Equal(userCountBeforeDelete - 1, await context.User.CountAsync());
            }
        }
    }
}
