using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLockSystem.Data;
using SmartLockSystem.Data.Models;

namespace SmartLockSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LockEventsController:ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LockEventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostLockEvent([FromBody] LockEvent lockEvent)
        {
            lockEvent.Timestamp = DateTime.Now;
            _context.LockEvents.Add(lockEvent);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Lock event recorded." });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LockEvent>>> GetLockEvents()
        {
            var logs = await _context.LockEvents
                .OrderByDescending(x => x.Id)
                .Select(x => new LogsresponseLogs()
                {
                    Status=x.Status,
                    Timestamp=x.Timestamp
                }).ToListAsync();
            return Ok( new LogsResponse
            {
                Logs = logs
            });
        }
        public class LogsResponse
        {
            public List<LogsresponseLogs> Logs { get; set; }
        }
        public class LogsresponseLogs
        {
            public string Status { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}
