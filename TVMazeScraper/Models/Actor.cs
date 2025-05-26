namespace TVMazeScraper.Models;

public class Actor
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateOnly? Birthday { get; set; }
    public long LastUpdated { get; set; }
    public ICollection<ShowActor> ShowActors { get; set; }
}