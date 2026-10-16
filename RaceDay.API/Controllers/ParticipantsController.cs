using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;

namespace RaceDay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ParticipantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/participants/{participantId}/enrolments
        [HttpGet("{participantId}/enrolments")]
        public async Task<IActionResult> GetEnrolments(int participantId)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Participant")
            {
                return StatusCode(403, new { message = "Access denied. You can only view your own data." });
            }

            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.ParticipantID == participantId);

            if (participant == null)
            {
                return NotFound(new { message = "Participant does not exist" });
            }

            // Participants can only view their own enrolments
            if (participant.UserID != userId)
            {
                return StatusCode(403, new { message = "Access denied. You can only view your own data." });
            }

            var enrolments = await _context.Enrolments
                .Where(e => e.ParticipantID == participantId)
                .Include(e => e.Event)
                .Include(e => e.Category)
                .Include(e => e.Result)
                .ToListAsync();

            return Ok(enrolments);
        }

        // GET: api/participants/{participantId}/results
        [HttpGet("{participantId}/results")]
        public async Task<IActionResult> GetResults(int participantId)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Participant")
            {
                return StatusCode(403, new { message = "Access denied. You can only view your own data." });
            }

            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.ParticipantID == participantId);

            if (participant == null)
            {
                return NotFound(new { message = "Participant does not exist" });
            }

            // Participants can only view their own results
            if (participant.UserID != userId)
            {
                return StatusCode(403, new { message = "Access denied. You can only view your own data." });
            }

            var results = await _context.Results
                .Include(r => r.Enrolment)
                .ThenInclude(e => e!.Event)
                .Where(r => r.Enrolment!.ParticipantID == participantId)
                .ToListAsync();

            return Ok(results);
        }
    }
}