using LogsManagment.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LogsManagment.Data
{
    public class AppContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        public AppContext(DbContextOptions<AppContext> options) : base(options)
        {
        }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<UserLog> UserLogs { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(AppContext).Assembly);
            base.OnModelCreating(builder);
        }
    }
}
