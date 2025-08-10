using API.Entities;
using API.Entities.Comments;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext(DbContextOptions options) : IdentityDbContext<User>(options)
{
    public DbSet<Post> Posts { get; init; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Reply> Replies { get; set; }
    public DbSet<Topic> Topics { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityRole>()
            .HasData(
                new IdentityRole { Name = "Admin", NormalizedName = "ADMIN"},
                new IdentityRole { Name = "None", NormalizedName = "NONE"}
                );

        modelBuilder.Entity<Post>()
            .HasMany(p => p.Topics)
            .WithMany(t => t.Posts)
            .UsingEntity(j => j.ToTable("PostTopics"));
        
        modelBuilder.Entity<Post>()
            .HasMany(a => a.MainComments)
            .WithOne()
            .HasForeignKey(c => c.PostId);
    }
}