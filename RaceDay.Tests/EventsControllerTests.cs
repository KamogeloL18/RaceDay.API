using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Controllers;
using RaceDay.API.Data;
using RaceDay.API.Models;
using Xunit;

namespace RaceDay.Tests
{
    public class EventsControllerTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private EventsController GetController(ApplicationDbContext context, int? userId = null, string? role = null)
        {
            var controller = new EventsController(context);
            var httpContext = new DefaultHttpContext();

            // ALWAYS set up a session for testing
            httpContext.Session = new TestSession();

            if (userId.HasValue)
            {
                httpContext.Session.SetInt32("UserID", userId.Value);
                httpContext.Session.SetString("Role", role ?? "Participant");
            }

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            return controller;
        }

        [Fact]
        public async Task GetAllEvents_ReturnsOkWithEvents()
        {
            var context = GetDbContext();
            context.Users.Add(new User { UserID = 1, Email = "org@test.com", Role = "Organiser", FullName = "Org", PasswordHash = "x" });
            context.Organisers.Add(new Organiser { OrganiserID = 1, UserID = 1 });
            context.Events.Add(new Event
            {
                EventID = 1,
                OrganiserID = 1,
                Name = "Test Event",
                Description = "Test",
                EventDate = DateTime.Now.AddDays(30),
                Location = "Test",
                Distance = 42.2m,
                EventType = "Run",
                IsActive = true
            });
            await context.SaveChangesAsync();

            var controller = GetController(context);

            var result = await controller.GetAllEvents();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task CreateEvent_AsOrganiser_ReturnsCreated()
        {
            var context = GetDbContext();
            context.Users.Add(new User { UserID = 1, Email = "org@test.com", Role = "Organiser", FullName = "Org", PasswordHash = "x" });
            context.Organisers.Add(new Organiser { OrganiserID = 1, UserID = 1 });
            await context.SaveChangesAsync();

            var controller = GetController(context, 1, "Organiser");
            var request = new CreateEventRequest
            {
                Name = "New Event",
                Description = "Test event",
                EventDate = DateTime.Now.AddDays(30),
                Location = "Cape Town",
                Distance = 42.2m,
                EventType = "Run"
            };

            var result = await controller.CreateEvent(request);

            Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(1, context.Events.Count());
        }

        [Fact]
        public async Task CreateEvent_AsParticipant_Returns403()
        {
            var context = GetDbContext();
            var controller = GetController(context, 1, "Participant");
            var request = new CreateEventRequest
            {
                Name = "Unauthorized Event",
                Description = "Test",
                EventDate = DateTime.Now.AddDays(30),
                Location = "Test",
                Distance = 42.2m,
                EventType = "Run"
            };

            var result = await controller.CreateEvent(request);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, statusResult.StatusCode);
        }

        [Fact]
        public async Task CreateEvent_NotLoggedIn_Returns403()
        {
            var context = GetDbContext();
            var controller = GetController(context);  // no user session
            var request = new CreateEventRequest
            {
                Name = "Anonymous Event",
                Description = "Test",
                EventDate = DateTime.Now.AddDays(30),
                Location = "Test",
                Distance = 42.2m,
                EventType = "Run"
            };

            var result = await controller.CreateEvent(request);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, statusResult.StatusCode);
        }
    }
}