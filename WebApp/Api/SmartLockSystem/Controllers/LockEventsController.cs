using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLockSystem.Data;
using SmartLockSystem.Data.Models;
using Microsoft.AspNetCore.SignalR;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SmartLockSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LockEventsController:ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<LockHub> _hubContext;
        private readonly MqttClient _mqttClient;
        private readonly IConfiguration _configuration;

        public LockEventsController(ApplicationDbContext context, IHubContext<LockHub> hubContext, IConfiguration configuration)
        {
            _context = context;
            _hubContext = hubContext;
            _configuration = configuration;
            string brokerAddress = _configuration["MQTT:BrokerAddress"];
            int port = int.Parse(_configuration["MQTT:Port"]);
            string clientId = _configuration["MQTT:ClientId"];
            string username = _configuration["MQTT:Username"];
            string password = _configuration["MQTT:Password"];

            // Initialize MQTT client with broker address and enable SSL
            _mqttClient = new MqttClient(brokerAddress, port, true, MqttSslProtocols.TLSv1_2, null, null);

            // Set event handler to confirm message publishing
            _mqttClient.MqttMsgPublished += MqttClient_MqttMsgPublished;

            // Connect to HiveMQ Cloud with credentials
            _mqttClient.Connect(clientId, username, password);

            // Subscribe to MQTT topics (e.g., lock/command) to receive commands from IoT devices
            _mqttClient.Subscribe(new string[] { "lock/command" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
        }
        // Event handler to confirm message publishing
        private void MqttClient_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            if (e.IsPublished)
            {
                Console.WriteLine("Message successfully published to MQTT with ID: " + e.MessageId);
            }
            else
            {
                Console.WriteLine("Message failed to publish to MQTT.");
            }
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

            // Publish to lock/state topic on HiveMQ
            var payload = System.Text.Encoding.UTF8.GetBytes(lockEvent.Status);
            _mqttClient.Publish("lock/state", payload, MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, retain: false);

            return Ok(new { message = "Lock event recorded and published to MQTT (HiveMQ)." });
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
