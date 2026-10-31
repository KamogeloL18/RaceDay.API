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

        // GET: api/participants/my-enrolments
        [HttpGet("my-enrolments")]
        public async Task<IActionResult> GetMyEnrolments()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Participant")
            {
                return StatusCode(403, new { message = "Access denied. Only Participants can view their enrolments." });
            }

            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.UserID == userId);

            if (participant == null)
            {
                return NotFound(new { message = "Participant profile not found." });
            }

            var enrolments = await _context.Enrolments
                .Where(e => e.ParticipantID == participant.ParticipantID)
                .Include(e => e.Event)
                .Include(e => e.Category)
                .Include(e => e.Result)
                .ToListAsync();

            return Ok(enrolments);
        }

        // GET: api/participants/my-results
        [HttpGet("my-results")]
        public async Task<IActionResult> GetMyResults()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Participant")
            {
                return StatusCode(403, new { message = "Access denied. Only Participants can view their results." });
            }

            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.UserID == userId);

            if (participant == null)
            {
                return NotFound(new { message = "Participant profile not found." });
            }

            var results = await _context.Results
                .Include(r => r.Enrolment)
                .ThenInclude(e => e!.Event)
                .Where(r => r.Enrolment!.ParticipantID == participant.ParticipantID)
                .ToListAsync();

            return Ok(results);
        }
    }
}