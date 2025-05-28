using Newtonsoft.Json;

namespace TVMazeScraper.Models;

public class ShowDTO
{
    public ShowDTO(Show show)
    {
        Id = show.Id;
        Title = show.Title;
        Actors = show.ShowRoles
            .OrderByDescending(a => a.Actor.Birthday)
            .Select(a => new ActorDTO(a.Actor))
            .ToList();
    }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Title { get; set; }

    [JsonProperty("cast")]
    public ICollection<ActorDTO> Actors { get; set; }
}
