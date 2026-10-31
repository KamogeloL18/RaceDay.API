using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlobService _blobService; // <-- ADDED

        public EventsController(ApplicationDbContext context, IBlobService blobService) // <-- ADDED
        {
            _context = context;
            _blobService = blobService;
        }

        // GET: api/events
        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            var query = _context.Events.Where(e => e.IsActive).AsQueryable();

            if (role == "Organiser" && userId.HasValue)
            {
                var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.UserID == userId);
                if (organiser != null)
                {
                    query = query.Where(e => e.OrganiserID == organiser.OrganiserID);
                }
            }

            var events = await query
                .Include(e => e.Organiser)
                .Include(e => e.Categories)
                .ToListAsync();

            return Ok(events);
        }

        // GET: api/events/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var eventItem = await _context.Events
                .Include(e => e.Organiser)
                .Include(e => e.Categories)
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (eventItem == null)
            {
                return NotFound(new { message = "Event does not exist" });
            }

            return Ok(eventItem);
        }

        // POST: api/events
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromForm] CreateEventRequest request)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
            {
                return StatusCode(403, new { message = "Access denied. Only Organisers can manage events." });
            }

            var organiser = await _context.Organisers
                .FirstOrDefaultAsync(o => o.UserID == userId);

            if (organiser == null)
            {
                return StatusCode(403, new { message = "Organiser profile not found." });
            }

            // UPLOAD TO AZURE BLOB STORAGE
            string? bannerImageUrl = null;
            if (request.BannerImage != null && request.BannerImage.Length > 0)
            {
                bannerImageUrl = await _blobService.UploadFileAsync(request.BannerImage);
            }

            var eventItem = new Event
            {
                OrganiserID = organiser.OrganiserID,
                Name = request.Name,
                Description = request.Description,
                EventDate = request.EventDate,
                Location = request.Location,
                Distance = request.Distance,
                EventType = request.EventType,
                BannerImage = bannerImageUrl,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { id = eventItem.EventID }, eventItem);
        }

        // PUT: api/events/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromForm] CreateEventRequest request)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
            {
                return StatusCode(403, new { message = "Access denied. Only Organisers can manage events." });
            }

            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
            {
                return NotFound(new { message = "Event does not exist" });
            }

            var organiser = await _context.Organisers
                .FirstOrDefaultAsync(o => o.UserID == userId);

            if (organiser == null || eventItem.OrganiserID != organiser.OrganiserID)
            {
                return StatusCode(403, new { message = "Access denied. You can only edit your own events." });
            }

            eventItem.Name = request.Name;
            eventItem.Description = request.Description;
            eventItem.EventDate = request.EventDate;
            eventItem.Location = request.Location;
            eventItem.Distance = request.Distance;
            eventItem.EventType = request.EventType;

            // UPLOAD NEW IMAGE TO AZURE BLOB STORAGE IF PROVIDED
            if (request.BannerImage != null && request.BannerImage.Length > 0)
            {
                eventItem.BannerImage = await _blobService.UploadFileAsync(request.BannerImage);
            }

            await _context.SaveChangesAsync();

            return Ok(eventItem);
        }

        // DELETE: api/events/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
            {
                return StatusCode(403, new { message = "Access denied. Only Organisers can manage events." });
            }

            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
            {
                return NotFound(new { message = "Event does not exist" });
            }

            var organiser = await _context.Organisers
                .FirstOrDefaultAsync(o => o.UserID == userId);

            if (organiser == null || eventItem.OrganiserID != organiser.OrganiserID)
            {
                return StatusCode(403, new { message = "Access denied. You can only delete your own events." });
            }

            eventItem.IsActive = false; // Soft delete
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class CreateEventRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public string EventType { get; set; } = string.Empty;
        public IFormFile? BannerImage { get; set; }
    }
}