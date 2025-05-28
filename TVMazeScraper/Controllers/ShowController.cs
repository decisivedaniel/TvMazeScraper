using Microsoft.AspNetCore.Mvc;
using TVMazeScraper.Services;
using TVMazeScraper.Models;

namespace TVMazeScraper.Controllers;

[ApiController]
[Route("[controller]")]
public class ShowController : ControllerBase
{
    private readonly ILogger<ShowController> _logger;
    private readonly IShowService _service;

    public ShowController(IShowService service, ILogger<ShowController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet(Name = "getshows/{page?}/{amount?}")]
    public async Task<ActionResult<IEnumerable<ShowDTO>>> GetShows(int page = 0, int amount = 100)
    {
        var shows = await _service.GetPageAsync(page, amount);
        if (shows.Count == 0) return NoContent();
        return shows.Select(s => new ShowDTO(s)).ToList();
    }
}
