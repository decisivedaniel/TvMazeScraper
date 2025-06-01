using TVMazeScraper.Models;
using System.Diagnostics;
using TVMazeScraper.Models.Json;
using System.Net;

namespace TVMazeScraper.Services;

public class ScraperService : IHostedService, IDisposable
{
    //In seconds (Default to 1 day)
    public long UpdateWindow { get; set; } = TimeSpan.FromDays(1).Seconds;
    private readonly TvMazeApiService _client;
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly ILogger<ScraperService> _logger;
    private CancellationTokenSource _stoppingCts = new CancellationTokenSource();
    private Timer _timer;

    public ScraperService(TvMazeApiService client, IServiceScopeFactory serviceFactory, ILogger<ScraperService> logger)
    {
        _client = client;
        _scopeFactory = serviceFactory;
        _logger = logger;
    }

    public async void ScrapingRequest(object state)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var showService = scope.ServiceProvider.GetRequiredService<IShowService>();
            var shouldUpdate = await CheckStorage(showService);
            if (shouldUpdate)
            {
                var sw = new Stopwatch();
                sw.Start();
                _logger.LogInformation("Updating");
                try
                {
                    await UpdateStorage(showService);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed at updating storage");
                }
                sw.Stop();
                _logger.LogInformation("Update took {1}", sw.Elapsed);
            }
        }
    }

    public async Task<bool> CheckStorage(IShowService showService)
    {
        _logger.LogInformation("Started Check");
        //get last time
        var lastUpdated = await showService.GetLastUpdatedValue();
        _logger.LogDebug("lastUpdated = {0}", lastUpdated);
        //check against requested time
        return (DateTimeOffset.Now.ToUnixTimeSeconds() - lastUpdated) >= UpdateWindow;
    }

    public async Task UpdateStorage(IShowService showService)
    {
        _logger.LogInformation("Started Update");
        var updateTask = _client.getShowUpdate();
        var lastUpdatedTask = showService.GetLastUpdatedValue();
        updateTask.Wait();
        var showList = updateTask.Result;
        lastUpdatedTask.Wait();
        var lastUpdated = lastUpdatedTask.Result;
        showList = showList
            .Where(x => x.Value >= lastUpdated)
            .OrderBy(x => x.Value)
            .ToDictionary();
        //get each show individually
        foreach (var showPair in showList)
        {
            if (_stoppingCts.IsCancellationRequested) break;
            var showInfo = await _client.getShow(showPair.Key);
            if (showInfo == null) continue;
            _logger.LogInformation("The show is call {Name}", showInfo.Name);
            var retrivedShow = new Show()
            {
                Id = showInfo.Id,
                Title = showInfo.Name,
                LastUpdated = showInfo.Updated ?? 0,
                ShowRoles = new List<Role>()
            };
            var actors = new List<Actor>();
            try
            {
                var searchList = showInfo.Embedded.Cast.DistinctBy(cast => cast.Person.Id);
                foreach (var cast in searchList)
                {
                    var person = cast.Person;
                    actors.Add(new Actor()
                    {
                        Id = person.Id,
                        Name = person.Name,
                        Birthday = person.Birthday,
                        LastUpdated = person.Updated ?? 0
                    });
                }
            }
            catch (NullReferenceException ex)
            {
                _logger.LogError(ex, "Embedded Issue");
                _logger.LogError("showinfo is {ShowInfo} with id {Id}", showInfo?.Embedded?.Cast, showPair.Key);
                throw;
            }

            await showService.CreateOrUpdateAsync(retrivedShow, actors);
        }
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        _timer = new Timer(ScrapingRequest, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(UpdateWindow));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service is stopping.");

        _stoppingCts.Cancel();

        _timer?.Change(Timeout.Infinite, 0);

        Task.Delay(TimeSpan.FromSeconds(2)).Wait();

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}

