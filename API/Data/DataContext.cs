using API.Entities;
using API.Entities.Comments;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users { get; init; } 
    public DbSet<Post> Posts { get; init; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Reply> Replies { get; set; }
    public DbSet<Topic> Topics { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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