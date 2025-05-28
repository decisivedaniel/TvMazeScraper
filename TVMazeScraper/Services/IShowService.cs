using TVMazeScraper.Models;

namespace TVMazeScraper.Services;

public interface IShowService
{
    Task<List<Show>> GetAllAsync();
    Task<List<Show>> GetPageAsync(int pageNumber = 0, int pageSize = 250);
    Task<long> GetLastUpdatedValue();
    Task CreateOrUpdateAsync(Show show, List<Actor> actors);
}