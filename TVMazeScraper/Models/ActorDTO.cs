using Newtonsoft.Json;

namespace TVMazeScraper.Models;

public class ActorDTO
{
    public ActorDTO(Actor actor)
    {
        Id = actor.Id;
        Name = actor.Name;
        Birthday = actor.Birthday;
    }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("birthday")]
    public string? Birthday { get; set; }
}