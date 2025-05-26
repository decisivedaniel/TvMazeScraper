namespace TVMazeScraper.Models;

public class Show
{
    public int Id { get; set; }
    public string Title { get; set; }
    public long LastUpdated { get; set; }
    public ICollection<ShowActor> ShowActors { get; set; }
}