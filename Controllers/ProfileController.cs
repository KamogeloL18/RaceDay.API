using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;

namespace RaceDay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/profile
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return Unauthorized(new { message = "Not logged in" });
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(new
            {
                userID = user.UserID,
                email = user.Email,
                fullName = user.FullName,
                role = user.Role,
                phoneNumber = user.PhoneNumber,
                createdAt = user.CreatedAt
            });
        }

        // PUT: api/profile
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return Unauthorized(new { message = "Not logged in" });
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            if (!string.IsNullOrEmpty(request.FullName))
                user.FullName = request.FullName;

            if (!string.IsNullOrEmpty(request.PhoneNumber))
                user.PhoneNumber = request.PhoneNumber;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                userID = user.UserID,
                email = user.Email,
                fullName = user.FullName,
                role = user.Role,
                phoneNumber = user.PhoneNumber
            });
        }
    }

    public class UpdateProfileRequest
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}