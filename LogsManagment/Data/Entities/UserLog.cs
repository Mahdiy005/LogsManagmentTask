namespace LogsManagment.Data.Entities
{
    public class UserLog
    {
        public int UserId { get; set; }
        public int LogId { get; set; }

        public bool Seen { get; set; } = false;

        // Navigation Property
        public virtual AppUser User { get; set; }
        public virtual Log Log { get; set; }
    }
}
