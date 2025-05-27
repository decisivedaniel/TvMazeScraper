using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
        var dbActors = new List<EntityEntry<Actor>>();
        actors.ForEach(actor =>
        {
            var existingActor = _context.Actors.SingleOrDefault(a => a.Id == actor.Id);
            var dbActor = existingActor == null ? _context.Actors.Add(actor) : _context.Actors.Update(actor);
            dbActors.Add(dbActor);
        });
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Problem with Saving: {0}", ex.Message);
            throw;
        }
        var existingShow = await _context.Shows.SingleOrDefaultAsync(s => s.Id == show.Id);
        var dbShow = existingShow == null ? _context.Shows.Add(show) : _context.Shows.Update(show);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Problem with Saving: {0}", ex.Message);
            throw;
        }
        dbActors.ForEach(actor =>
        {
            dbShow.Entity.ShowRoles.Add(new Role
            {
                Show = dbShow.Entity,
                Actor = actor.Entity
            });
        });
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Problem with Saving: {0}", ex.Message);
            throw;
        }
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
        try
        {
            var showsUpdated = await _context.Shows.Select(x => x.LastUpdated).ToListAsync();
            return showsUpdated.Count == 0 ? 0 : showsUpdated.Max();
        }
        catch (Exception ex)
        {
            _logger.LogError("Problem with " + ex.Message);
            throw;
        }
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