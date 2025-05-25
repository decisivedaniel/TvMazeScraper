using TVMazeScraper.Models;

namespace TVMazeScraper.Services;

interface IShowService
{
    Task<List<Show>> GetAllAsync();
    Task<List<Show>> GetPageAsync(int pageNumber = 0, int pageSize = 250);
    Task CreateAsync(Show newShow);
    Task UpdateAsync(Show updatedShow);
}