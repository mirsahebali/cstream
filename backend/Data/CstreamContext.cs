using Microsoft.EntityFrameworkCore;
using Cstream.Models;

namespace Cstream.Data;

public class CstreamContext : DbContext
{
    public CstreamContext(DbContextOptions<CstreamContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<VideoStream> VideoStreams { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<VideoStream>().ToTable("VideoStream");
    }
}
