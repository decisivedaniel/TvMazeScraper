using Microsoft.EntityFrameworkCore;
using TVMazeScraper.Data;
using TVMazeScraper.Models;

namespace TVMazeScraper.Services;

public class SqliteShowService : IShowService
{
    private readonly ProgramContext _context;

    public SqliteShowService(ProgramContext programContext)
    {
        _context = programContext;
    }

    public Task CreateAsync(Show newShow)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Show>> GetAllAsync()
    {
        return await _context.Shows
            .Include(show =>
                show.ShowActors.Select(showActor => showActor.Actor).OrderBy(x => x.Birthday))
            .ToListAsync();
    }

    public async Task<long> GetLastUpdatedValue()
    {
        return await _context.Shows.Select(x => x.LastUpdated).DefaultIfEmpty(0).MaxAsync();
    }

    public async Task<List<Show>> GetPageAsync(int pageNumber = 0, int pageSize = 250)
    {
        var result = await GetAllAsync();
        return result.Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToList();  
    }

    public Task UpdateAsync(Show updatedShow)
    {
        throw new NotImplementedException();
    }
}