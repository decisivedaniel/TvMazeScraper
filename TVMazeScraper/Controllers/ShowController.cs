using Microsoft.AspNetCore.Mvc;
using TVMazeScraper.Services;
using TVMazeScraper.Models;

namespace TVMazeScraper.Controllers;

[ApiController]
[Route("[controller]")]
public class ShowController : ControllerBase
{
    private readonly int PageMin = 0;
    private readonly int PageAmountMin = 50;
    private readonly ILogger<ShowController> _logger;
    private readonly IShowService _service;

    public ShowController(IShowService service, ILogger<ShowController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet("{page?}/{amount?}", Name = "Show_Pages")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ShowDTO>>> GetShows(int page = 0, int amount = 100)
    {
        if (page < PageMin) return BadRequest($"Page number can't be below {PageMin}");
        if (amount < PageAmountMin) return BadRequest($"Keep each page to at least {PageAmountMin}");
        var shows = await _service.GetPageAsync(page, amount);
        if (shows.Count == 0) return NoContent();
        return shows.Select(s => new ShowDTO(s)).ToList();
    }
}
