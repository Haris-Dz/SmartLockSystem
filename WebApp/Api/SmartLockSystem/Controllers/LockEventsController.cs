using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLockSystem.Data;
using SmartLockSystem.Data.Models;
using Microsoft.AspNetCore.SignalR;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Threading.Tasks;

namespace SmartLockSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LockEventsController:ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<LockHub> _hubContext;
        private readonly MqttClient _mqttClient;

        public LockEventsController(ApplicationDbContext context, IHubContext<LockHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
            // Connect to MQTT broker
            //_mqttClient = new MqttClient("your-mqtt-broker-address");
            //_mqttClient.Connect("clientId", "username", "password");
        }

        [HttpPost("IOT")]
        public async Task<IActionResult> PostLockEventIOT([FromBody] LockEvent lockEvent)
        {
            var action = new LockEvent
            {
                Status = lockEvent.Status,
                Timestamp = DateTime.Now,
            };
            lockEvent.Timestamp = DateTime.Now;
            _context.LockEvents.Add(action);
            await _context.SaveChangesAsync();
            // Notify all Angular clients via SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveLockUpdate", lockEvent.Status);

            return Ok(new { message = "Lock event recorded." });
        }
        [HttpPost("WebApp")]
        public async Task<IActionResult> PostLockEventWeb([FromBody] LockEvent lockEvent)
        {
            var action = new LockEvent
            {
                Status = lockEvent.Status,
                Timestamp = DateTime.Now,
            };
            lockEvent.Timestamp = DateTime.Now;
            _context.LockEvents.Add(action);
            await _context.SaveChangesAsync();

            // Notify the NodeMCU via MQTT
            //_mqttClient.Publish("lock/state", System.Text.Encoding.UTF8.GetBytes(lockEvent.Status));

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
        public class LockHub : Hub
        {
            // Send the lock state update to all connected clients
            public async Task SendLockUpdate(string state, string username)
            {
                await Clients.All.SendAsync("ReceiveLockUpdate", state, username);
            }
        }
    }
}
