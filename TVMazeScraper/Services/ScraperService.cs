using Newtonsoft.Json;
using TVMazeScraper.Models;
using TVMazeScraper.Models.Json;
using System.Diagnostics;
using System.Net;

namespace TVMazeScraper.Services;

public class ScraperService : IHostedService, IDisposable
{
    //In seconds (Default to 1 day)
    public long UpdateWindow { get; set; } = 60 * 60 * 24;
    private readonly TvMazeHttpClient _client;
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly ILogger<ScraperService> _logger;
    private Timer _timer;

    public ScraperService(TvMazeHttpClient httpClient, IServiceScopeFactory serviceFactory, ILogger<ScraperService> logger)
    {
        _client = httpClient;
        _scopeFactory = serviceFactory;
        _logger = logger;
    }

    public async void ScrapingRequest(object state)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var showService = scope.ServiceProvider.GetRequiredService<IShowService>();
            var shouldUpdate = await CheckStorage(showService);
            _logger.LogInformation("ShouldUpdate is {0}", shouldUpdate);
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
        var updateResponse = await _client.GetAsync("https://api.tvmaze.com/updates/shows");
        if (!updateResponse.IsSuccessStatusCode)
        {
            _logger.LogError("Didn't succeed in getting updated show list with code: " + updateResponse.StatusCode);
        }
        var jsonStringTask = updateResponse.Content.ReadAsStringAsync();
        var lastUpdatedTask = showService.GetLastUpdatedValue();
        jsonStringTask.Wait();
        var jsonString = jsonStringTask.Result;
        Dictionary<long, long> showList;
        try
        {
            showList = JsonConvert.DeserializeObject<Dictionary<long, long>>(jsonString);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Serializer error from updates/shows");
            return;
        }
        lastUpdatedTask.Wait();
        var lastUpdated = lastUpdatedTask.Result;
        showList = showList.Where(x => x.Value > lastUpdated).ToDictionary();

        //get each show individually
        foreach (var showPair in showList)
        {
            var showResponse = await _client.GetAsync($"https://api.tvmaze.com/shows/{showPair.Key}?embed=cast");
            var resultJson = await showResponse.Content.ReadAsStringAsync();
            ShowRoot? showInfo;
            try
            {
                showInfo = JsonConvert.DeserializeObject<ShowRoot>(resultJson);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Json Serialziation Error from show {Id}", showPair.Key);
                throw;
            }
            if (showInfo == null) return;
            var retrivedShow = new Show()
            {
                Id = showInfo.Id,
                Title = showInfo.Name,
                LastUpdated = showInfo.Updated ?? DateTimeOffset.Now.ToUnixTimeSeconds(),
                ShowRoles = new List<Role>()
            };
            var actors = new List<Actor>();
            try
            {
                foreach (var cast in showInfo.Embedded.Cast)
                {
                    var person = cast.Person;
                    actors.Add(new Actor()
                    {
                        Id = person.Id,
                        Birthday = person.Birthday,
                        LastUpdated = person.Updated ?? DateTimeOffset.Now.ToUnixTimeSeconds()
                    });
                }
            }
            catch (NullReferenceException ex)
            {
                _logger.LogError(ex, "Embedded Issue");
                _logger.LogInformation("showinfo is {showInfo} with id {Id}", showInfo, showInfo.Id);
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

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}

// To handle too many requests responses gracefully
public class TvMazeHttpClient
{
    private readonly HttpClient _client;
    private readonly ILogger<TvMazeHttpClient> _logger;
    public TvMazeHttpClient(HttpClient httpClient, ILogger<TvMazeHttpClient> logger)
    {
        _client = httpClient;
        _logger = logger;
    }
    public async Task<HttpResponseMessage> GetAsync(string url)
    {
        var response = await _client.GetAsync(url);
        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            var t = Task.Run(async delegate
            {
                await Task.Delay(TimeSpan.FromSeconds(2).Milliseconds);
            });
            t.Wait();
            response = await GetAsync(url);
        }
        else if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Didn't return proper value from url {url}", url);
        }
        
        return response;
    }
}