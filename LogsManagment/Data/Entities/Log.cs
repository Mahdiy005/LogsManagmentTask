namespace LogsManagment.Data.Entities
{
    public class Log
    {
        public Log()
        {
            UsersAssigned = new HashSet<UserLog>();
            Comments = new HashSet<Comment>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        //public bool IsSeen { get; set; } // Relation Attribute
        public DateTime CreatedAt { get; set; }

        // Foreign Key
        public int AdminId { get; set; } // who admin created that log

        // Navigation property
        public virtual AppUser AdminWhoCreated { get; set; }
        public virtual ICollection<UserLog> UsersAssigned { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

    }
}
