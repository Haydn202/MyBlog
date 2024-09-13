using API.Entities;
using API.Entities.Comments;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User?> Users { get; init; } 
    public DbSet<Post> Posts { get; init; }
    public DbSet<MainComment> MainComments { get; set; }
    public DbSet<SubComment> SubComments { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public DbSet<Contributor> Contributors { get; set; }
}