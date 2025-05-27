using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using TVMazeScraper.Data;
using TVMazeScraper.Models;

namespace TVMazeScraper.Services;

public class SqliteShowService : IShowService
{
    private readonly ProgramContext _context;
    private readonly ILogger<SqliteShowService> _logger;

    public SqliteShowService(ProgramContext programContext, ILogger<SqliteShowService> logger)
    {
        _context = programContext;
        _logger = logger;
    }

    public async Task CreateOrUpdateAsync(Show show, List<Actor> actors)
    {
        actors.ForEach(actor =>
        {
            var existingActor = _context.Actors.SingleOrDefault(a => a.Id == actor.Id);
            _ = existingActor == null ? _context.Actors.Add(actor) : _context.Actors.Update(actor);
        });
        await _context.SaveChangesAsync();
        var existingShow = await _context.Shows.SingleOrDefaultAsync(s => s.Id == show.Id);
        _ = existingShow == null ? _context.Shows.Add(show) : _context.Shows.Update(show);
        await _context.SaveChangesAsync();
        existingShow = await _context.Shows.SingleAsync(s => s.Id == show.Id);
        actors.ForEach(actor =>
        {
            existingShow.ShowRoles.Add(new Role
            {
                Show = existingShow,
                Actor = actor
            });
        });
        await _context.SaveChangesAsync();
    }

    public async Task<List<Show>> GetAllAsync()
    {
        return await _context.Shows
            .Include(show => show.ShowRoles.OrderBy(x => x.Actor.Birthday))
            .ThenInclude(role => role.Actor)
            .ToListAsync();
    }

    public async Task<long> GetLastUpdatedValue()
    {
        long value;
        try
        {
            value = await _context.Shows.Select(x => x.LastUpdated).MaxAsync();
        }
        catch (InvalidOperationException)
        {
            value = 0;
        }
        catch (Exception ex)
        {
            _logger.LogError("Problem with {1}", ex.Message);
            throw;
        }
        return value;
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