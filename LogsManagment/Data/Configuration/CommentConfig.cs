using LogsManagment.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogsManagment.Data.Configuration
{
    public class CommentConfig : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(c => c.Id);
            builder.HasOne(c => c.Log)
                   .WithMany(l => l.Comments)
                   .HasForeignKey(c => c.LogId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.User)
                   .WithMany(u => u.Comments)
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
