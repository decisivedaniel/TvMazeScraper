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
                show.ShowActors.Select(showActor => showActor.Actor))
            .ToListAsync();
    }

    public async Task<long> GetLastUpdatedValue()
    {
        return await _context.Shows.Select(x => x.LastUpdated).DefaultIfEmpty().MaxAsync();
    }

    public Task<List<Show>> GetPageAsync(int pageNumber = 0, int pageSize = 250)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Show updatedShow)
    {
        throw new NotImplementedException();
    }
}