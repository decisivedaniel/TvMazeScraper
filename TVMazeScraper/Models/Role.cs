namespace TVMazeScraper.Models;

public class Role
{
    public int ShowId { get; set; }
    public int ActorId { get; set; }

    public Show Show { get; set; }
    public Actor Actor { get; set; }

}