using Microsoft.AspNetCore.Identity;

namespace LogsManagment.Data.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public AppUser()
        {
            CreatedAt = DateTime.UtcNow;
            CreatedLogs = new HashSet<Log>();
            AssignedLogs = new HashSet<UserLog>();
            Comments = new HashSet<Comment>();
        }

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<Log> CreatedLogs { get; set; } // Logs created by this user

        public virtual ICollection<UserLog> AssignedLogs { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<IdentityUserRole<int>> UserRoles { get; set; }
        public virtual List<RefreshToken>? RefreshTokens { get; set; }
    }
}
