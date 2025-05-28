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
        //using var transation = await _context.Database.BeginTransactionAsync();
        List<Actor> dbActors = new List<Actor>();
        actors.ForEach(async actor =>
        {
            var existingActor = await GetActorAsync(actor.Id);
            if (existingActor == null)
            {
                _context.Actors.Add(actor);
                dbActors.Add(actor);
            }
            else
            {
                existingActor.Birthday = actor.Birthday;
                existingActor.Name = actor.Name;
                existingActor.LastUpdated = actor.LastUpdated;
                _context.Actors.Update(existingActor);
                dbActors.Add(actor);
            } 
        });
        try
        {
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
        }
        catch (Exception ex)
        {
            _logger.LogError("Problem with Saving: {0}", ex.Message);
            throw;
        }
        var possibleShow = await GetShowAsync(show.Id);
        Show existingShow;
         _logger.LogInformation("ShowRole is {role}", show.ShowRoles);
        if (possibleShow == null)
        {
            _context.Shows.Add(show);
            existingShow = show;
        }
        else
        {
            existingShow = possibleShow;
            existingShow.Title = show.Title;
            existingShow.LastUpdated = show.LastUpdated;
            _context.Shows.Update(possibleShow);
        }
        try
        {
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
        }
        catch (Exception ex)
        {
            _logger.LogError("Problem with Saving: {0}", ex.Message);
            throw;
        }
        _logger.LogInformation("ShowRole is {role}", existingShow.ShowRoles);
        dbActors.ForEach(actor =>
        {
            existingShow.ShowRoles.Add(new Role
            {
                Show = show,
                Actor = actor
            });
        });
        try
        {
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
        }
        catch (Exception ex)
        {
            _logger.LogError("Problem with Saving: {0}", ex.Message);
            throw;
        }
    }

    public async Task<Actor?> GetActorAsync(int id)
    {
        return await _context.Actors
            .Include(actor => actor.ActorRoles)
            .ThenInclude(role => role.Show)
            .SingleOrDefaultAsync(actor => actor.Id == id);
    }

    public async Task<Show?> GetShowAsync(int id)
    {
        return await _context.Shows
            .Include(show => show.ShowRoles)
            .ThenInclude(role => role.Actor)
            .SingleOrDefaultAsync(show => show.Id == id);
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