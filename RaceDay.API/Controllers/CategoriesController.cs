using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/events/{eventId}/category
        [HttpGet("events/{eventId}/category")]
        public async Task<IActionResult> GetCategories(int eventId)
        {
            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
            {
                return NotFound(new { message = "Event does not exist" });
            }

            var categories = await _context.Categories
                .Where(c => c.EventID == eventId)
                .ToListAsync();

            return Ok(categories);
        }

        // POST: api/events/{eventId}/category
        [HttpPost("events/{eventId}/category")]
        public async Task<IActionResult> CreateCategory(int eventId, [FromBody] CreateCategoryRequest request)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
            {
                return StatusCode(403, new { message = "Access denied. Only Organisers can manage categories." });
            }

            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
            {
                return NotFound(new { message = "Event does not exist" });
            }

            var organiser = await _context.Organisers
                .FirstOrDefaultAsync(o => o.UserID == userId);

            if (organiser == null || eventItem.OrganiserID != organiser.OrganiserID)
            {
                return StatusCode(403, new { message = "Access denied. Only Organisers can manage categories." });
            }

            var category = new Category
            {
                EventID = eventId,
                Name = request.Name,
                Description = request.Description,
                MinAge = request.MinAge,
                MaxAge = request.MaxAge,
                Distance = request.Distance,
                EntryFee = request.EntryFee,
                CreatedAt = DateTime.Now
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategories), new { eventId = eventId }, category);
        }

        // PUT: api/categories/{id}
        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryRequest request)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
            {
                return StatusCode(403, new { message = "Access denied. Only Organisers can manage categories." });
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new { message = "Category does not exist" });
            }

            var eventItem = await _context.Events.FindAsync(category.EventID);
            var organiser = await _context.Organisers
                .FirstOrDefaultAsync(o => o.UserID == userId);

            if (organiser == null || eventItem == null || eventItem.OrganiserID != organiser.OrganiserID)
            {
                return StatusCode(403, new { message = "Access denied. Only Organisers can manage categories." });
            }

            category.Name = request.Name;
            category.Description = request.Description;
            category.MinAge = request.MinAge;
            category.MaxAge = request.MaxAge;
            category.Distance = request.Distance;
            category.EntryFee = request.EntryFee;

            await _context.SaveChangesAsync();

            return Ok(category);
        }

        // DELETE: api/categories/{id}
        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
            {
                return StatusCode(403, new { message = "Access denied. Only Organisers can manage categories." });
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new { message = "Category does not exist" });
            }

            var eventItem = await _context.Events.FindAsync(category.EventID);
            var organiser = await _context.Organisers
                .FirstOrDefaultAsync(o => o.UserID == userId);

            if (organiser == null || eventItem == null || eventItem.OrganiserID != organiser.OrganiserID)
            {
                return StatusCode(403, new { message = "Access denied. Only Organisers can manage categories." });
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class CreateCategoryRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public decimal? Distance { get; set; }
        public decimal? EntryFee { get; set; }
    }
}