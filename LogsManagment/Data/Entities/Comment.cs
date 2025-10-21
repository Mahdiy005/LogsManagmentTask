namespace LogsManagment.Data.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Foreign keys
        public int LogId { get; set; }
        public int UserId { get; set; }
        // Navigation properties
        public virtual Log Log { get; set; } // which log this comment belongs to
        public virtual AppUser User { get; set; } // which user made the comment
    }
}
