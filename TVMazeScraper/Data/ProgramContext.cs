using TVMazeScraper.Models;
using Microsoft.EntityFrameworkCore;

namespace TVMazeScraper.Data;

public class ProgramContext : DbContext
{
    public ProgramContext(DbContextOptions<ProgramContext> options)
        : base(options) {}

    public DbSet<Show> Shows { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<ShowActor> ShowActors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShowActor>()
            .HasKey(c => new { c.ShowID, c.ActorID });
    }
}