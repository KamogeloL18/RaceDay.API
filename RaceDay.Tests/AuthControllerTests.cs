using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Controllers;
using RaceDay.API.Data;
using Xunit;

namespace RaceDay.Tests
{
    public class AuthControllerTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private AuthController GetController(ApplicationDbContext context)
        {
            var controller = new AuthController(context);
            var httpContext = new DefaultHttpContext();

            // Always set up a session for testing
            httpContext.Session = new TestSession();

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            return controller;
        }

        [Fact]
        public async Task Register_ValidOrganiser_ReturnsCreated()
        {
            var context = GetDbContext();
            var controller = GetController(context);
            var request = new RegisterRequest
            {
                Email = "organiser@test.com",
                Password = "Password123",
                FullName = "Test Organiser",
                Role = "Organiser"
            };

            var result = await controller.Register(request);

            Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(1, context.Users.Count());
            Assert.Equal("Organiser", context.Users.First().Role);
        }

        [Fact]
        public async Task Register_ValidParticipant_ReturnsCreated()
        {
            var context = GetDbContext();
            var controller = GetController(context);
            var request = new RegisterRequest
            {
                Email = "participant@test.com",
                Password = "Password123",
                FullName = "Test Participant",
                Role = "Participant",
                DateOfBirth = new DateTime(1990, 5, 15),
                Gender = "Male"
            };

            var result = await controller.Register(request);

            Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(1, context.Users.Count());
            Assert.Equal("Participant", context.Users.First().Role);
        }

        [Fact]
        public async Task Register_DuplicateEmail_ReturnsConflict()
        {
            var context = GetDbContext();
            var controller = GetController(context);
            var request = new RegisterRequest
            {
                Email = "duplicate@test.com",
                Password = "Password123",
                FullName = "First User",
                Role = "Participant",
                DateOfBirth = new DateTime(1990, 5, 15),
                Gender = "Male"
            };
            await controller.Register(request);

            var result = await controller.Register(request);

            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public async Task Register_InvalidRole_ReturnsBadRequest()
        {
            var context = GetDbContext();
            var controller = GetController(context);
            var request = new RegisterRequest
            {
                Email = "invalid@test.com",
                Password = "Password123",
                FullName = "Invalid User",
                Role = "Admin"
            };

            var result = await controller.Register(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOk()
        {
            var context = GetDbContext();
            var controller = GetController(context);
            await controller.Register(new RegisterRequest
            {
                Email = "login@test.com",
                Password = "Password123",
                FullName = "Login User",
                Role = "Participant",
                DateOfBirth = new DateTime(1990, 5, 15),
                Gender = "Male"
            });

            var result = await controller.Login(new LoginRequest
            {
                Email = "login@test.com",
                Password = "Password123"
            });

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Login_WrongPassword_ReturnsUnauthorized()
        {
            var context = GetDbContext();
            var controller = GetController(context);
            await controller.Register(new RegisterRequest
            {
                Email = "login@test.com",
                Password = "Password123",
                FullName = "Login User",
                Role = "Participant",
                DateOfBirth = new DateTime(1990, 5, 15),
                Gender = "Male"
            });

            var result = await controller.Login(new LoginRequest
            {
                Email = "login@test.com",
                Password = "WrongPassword"
            });

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Login_NonExistentUser_ReturnsUnauthorized()
        {
            var context = GetDbContext();
            var controller = GetController(context);

            var result = await controller.Login(new LoginRequest
            {
                Email = "nobody@test.com",
                Password = "Password123"
            });

            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}