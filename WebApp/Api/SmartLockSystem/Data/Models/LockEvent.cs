namespace SmartLockSystem.Data.Models
{
    public class LockEvent
    {
        public int Id { get; set; }
        public string Status { get; set; } // "locked" or "unlocked"
        public DateTime Timestamp { get; set; }
    }
}
