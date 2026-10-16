using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class EnrolmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EnrolmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/events/{eventId}/enrol
        [HttpPost("events/{eventId}/enrol")]
        public async Task<IActionResult> Enrol(int eventId, [FromBody] EnrolRequest request)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Participant")
            {
                return StatusCode(403, new { message = "Access denied. Insufficient permissions." });
            }

            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.UserID == userId);

            if (participant == null)
            {
                return StatusCode(403, new { message = "Access denied. Insufficient permissions." });
            }

            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
            {
                return NotFound(new { message = "Event does not exist" });
            }

            // Check if already enrolled
            var existing = await _context.Enrolments
                .AnyAsync(e => e.ParticipantID == participant.ParticipantID && e.EventID == eventId);

            if (existing)
            {
                return BadRequest(new { message = "Already enrolled in this event" });
            }

            // Check category exists
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryID == request.CategoryID && c.EventID == eventId);

            if (category == null)
            {
                return BadRequest(new { message = "Invalid category" });
            }

            var enrolment = new Enrolment
            {
                ParticipantID = participant.ParticipantID,
                EventID = eventId,
                CategoryID = request.CategoryID,
                EnrolmentDate = DateTime.Now,
                Status = "Pending",
                PaymentStatus = "Unpaid"
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Enrol), new { id = enrolment.EnrolmentID }, enrolment);
        }

        // GET: api/events/{eventId}/enrolments
        [HttpGet("events/{eventId}/enrolments")]
        public async Task<IActionResult> GetEventEnrolments(int eventId)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
            {
                return StatusCode(403, new { message = "Access denied. Insufficient permissions." });
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
                return StatusCode(403, new { message = "Access denied. Insufficient permissions." });
            }

            var enrolments = await _context.Enrolments
                .Where(e => e.EventID == eventId)
                .Include(e => e.Participant)
                .ThenInclude(p => p.User)
                .Include(e => e.Category)
                .Include(e => e.Result)
                .ToListAsync();

            return Ok(enrolments);
        }

        // PUT: api/enrolments/{id}/status
        [HttpPut("enrolments/{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
            {
                return StatusCode(403, new { message = "Access denied. Insufficient permissions." });
            }

            var enrolment = await _context.Enrolments
                .Include(e => e.Event)
                .FirstOrDefaultAsync(e => e.EnrolmentID == id);

            if (enrolment == null)
            {
                return NotFound(new { message = "Enrolment does not exist" });
            }

            var organiser = await _context.Organisers
                .FirstOrDefaultAsync(o => o.UserID == userId);

            if (organiser == null || enrolment.Event == null || enrolment.Event.OrganiserID != organiser.OrganiserID)
            {
                return StatusCode(403, new { message = "Access denied. Insufficient permissions." });
            }

            enrolment.Status = request.Status;
            await _context.SaveChangesAsync();

            return Ok(enrolment);
        }

        // POST: api/enrolments/{id}/result
        [HttpPost("enrolments/{id}/result")]
        public async Task<IActionResult> RecordResult(int id, [FromBody] RecordResultRequest request)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
            {
                return StatusCode(403, new { message = "Access denied. Insufficient permissions." });
            }

            var enrolment = await _context.Enrolments
                .Include(e => e.Event)
                .FirstOrDefaultAsync(e => e.EnrolmentID == id);

            if (enrolment == null)
            {
                return NotFound(new { message = "Enrolment does not exist" });
            }

            var organiser = await _context.Organisers
                .FirstOrDefaultAsync(o => o.UserID == userId);

            if (organiser == null || enrolment.Event == null || enrolment.Event.OrganiserID != organiser.OrganiserID)
            {
                return StatusCode(403, new { message = "Access denied. Insufficient permissions." });
            }

            // Check if result already exists
            var existingResult = await _context.Results
                .FirstOrDefaultAsync(r => r.EnrolmentID == id);

            if (existingResult != null)
            {
                existingResult.FinishTime = request.FinishTime;
                existingResult.Position = request.Position;
                existingResult.IsCompleted = request.IsCompleted;
                existingResult.Notes = request.Notes;
            }
            else
            {
                var result = new Result
                {
                    EnrolmentID = id,
                    FinishTime = request.FinishTime,
                    Position = request.Position,
                    IsCompleted = request.IsCompleted,
                    Notes = request.Notes,
                    RecordedAt = DateTime.Now
                };
                _context.Results.Add(result);
            }

            // Update enrolment status
            if (request.IsCompleted)
            {
                enrolment.Status = "Completed";
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Result recorded successfully" });
        }
    }

    public class EnrolRequest
    {
        public int CategoryID { get; set; }
    }

    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }

    public class RecordResultRequest
    {
        public TimeSpan? FinishTime { get; set; }
        public int? Position { get; set; }
        public bool IsCompleted { get; set; }
        public string? Notes { get; set; }
    }
}