using LogsManagment.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogsManagment.Data.Configuration
{
    public class AppUserConfig : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.HasKey(u => u.Id);

            // Relationship: One AppUser (Admin) to Many Logs
            builder.HasMany(u => u.CreatedLogs)
                   .WithOne(l => l.AdminWhoCreated)
                   .HasForeignKey(l => l.AdminId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes
        }
    }
}
