using TVMazeScraper.Models;
using Microsoft.EntityFrameworkCore;

namespace TVMazeScraper.Data;

public class ProgramContext : DbContext
{
    public virtual DbSet<Show> Shows { get; set; }
    public virtual DbSet<Actor> Actors { get; set; }
    public virtual DbSet<ShowActor> ShowActors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShowActor>()
            .HasKey(c => new { c.ShowID, c.ActorID });
    }
}