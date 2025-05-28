using TVMazeScraper.Models;
using Microsoft.EntityFrameworkCore;

namespace TVMazeScraper.Data;

public class ProgramContext : DbContext
{
    public ProgramContext() { }
    public ProgramContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }
    public virtual DbSet<Show> Shows { get; set; }
    public virtual DbSet<Actor> Actors { get; set; }
    public virtual DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>()
            .HasKey(c => new { c.ShowId, c.ActorId });
    }
}