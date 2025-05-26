namespace TVMazeScraper.Models;

public class ShowActor
{
    public int ShowID { get; set; }
    public int ActorID { get; set; }
    public Show Show { get; set; }
    public Actor Actor { get; set; }
}