using LogsManagment.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogsManagment.Data.Configuration
{
    public class UserLogConfig : IEntityTypeConfiguration<UserLog>
    {
        public void Configure(EntityTypeBuilder<UserLog> builder)
        {
            builder.HasKey(ul => new { ul.UserId, ul.LogId });
            // Relationship: One User to Many UserLogs
            builder.HasOne(ul => ul.User)
                   .WithMany(u => u.AssignedLogs)
                   .HasForeignKey(ul => ul.UserId)
                   .OnDelete(DeleteBehavior.Cascade); // Cascade delete when a User is deleted
            // Relationship: One Log to Many UserLogs
            builder.HasOne(ul => ul.Log)
                   .WithMany(l => l.UsersAssigned)
                   .HasForeignKey(ul => ul.LogId)
                   .OnDelete(DeleteBehavior.Cascade); // Cascade delete when a Log is deleted
        }


    }
}
