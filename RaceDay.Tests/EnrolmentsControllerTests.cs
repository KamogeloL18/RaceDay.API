using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Controllers;
using RaceDay.API.Data;
using RaceDay.API.Models;
using Xunit;

namespace RaceDay.Tests
{
    public class EnrolmentsControllerTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private EnrolmentsController GetController(ApplicationDbContext context, int? userId = null, string? role = null)
        {
            var controller = new EnrolmentsController(context);
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
        public async Task Enrol_AsParticipant_ReturnsCreated()
        {
            var context = GetDbContext();
            context.Users.Add(new User { UserID = 1, Email = "p@test.com", Role = "Participant", FullName = "P", PasswordHash = "x" });
            context.Users.Add(new User { UserID = 2, Email = "o@test.com", Role = "Organiser", FullName = "O", PasswordHash = "x" });
            context.Participants.Add(new Participant { ParticipantID = 1, UserID = 1 });
            context.Organisers.Add(new Organiser { OrganiserID = 1, UserID = 2 });
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
            context.Categories.Add(new Category { CategoryID = 1, EventID = 1, Name = "Senior Men" });
            await context.SaveChangesAsync();

            var controller = GetController(context, 1, "Participant");

            var result = await controller.Enrol(1, new EnrolRequest { CategoryID = 1 });

            Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(1, context.Enrolments.Count());
        }

        [Fact]
        public async Task Enrol_AsOrganiser_Returns403()
        {
            var context = GetDbContext();
            var controller = GetController(context, 1, "Organiser");

            var result = await controller.Enrol(1, new EnrolRequest { CategoryID = 1 });

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, statusResult.StatusCode);
        }

        [Fact]
        public async Task Enrol_DuplicateEnrolment_ReturnsBadRequest()
        {
            var context = GetDbContext();
            context.Users.Add(new User { UserID = 1, Email = "p@test.com", Role = "Participant", FullName = "P", PasswordHash = "x" });
            context.Users.Add(new User { UserID = 2, Email = "o@test.com", Role = "Organiser", FullName = "O", PasswordHash = "x" });
            context.Participants.Add(new Participant { ParticipantID = 1, UserID = 1 });
            context.Organisers.Add(new Organiser { OrganiserID = 1, UserID = 2 });
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
            context.Categories.Add(new Category { CategoryID = 1, EventID = 1, Name = "Senior Men" });
            context.Enrolments.Add(new Enrolment { EnrolmentID = 1, ParticipantID = 1, EventID = 1, CategoryID = 1 });
            await context.SaveChangesAsync();

            var controller = GetController(context, 1, "Participant");

            var result = await controller.Enrol(1, new EnrolRequest { CategoryID = 1 });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Enrol_InvalidCategory_ReturnsBadRequest()
        {
            var context = GetDbContext();
            context.Users.Add(new User { UserID = 1, Email = "p@test.com", Role = "Participant", FullName = "P", PasswordHash = "x" });
            context.Users.Add(new User { UserID = 2, Email = "o@test.com", Role = "Organiser", FullName = "O", PasswordHash = "x" });
            context.Participants.Add(new Participant { ParticipantID = 1, UserID = 1 });
            context.Organisers.Add(new Organiser { OrganiserID = 1, UserID = 2 });
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

            var controller = GetController(context, 1, "Participant");

            var result = await controller.Enrol(1, new EnrolRequest { CategoryID = 999 });

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}