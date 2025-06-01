using System.Net;
using Newtonsoft.Json;
using Polly.Caching;
using TVMazeScraper.Models.Json;

namespace TVMazeScraper.Services;

public class TvMazeApiService
{
    private readonly HttpClient _client;
    private readonly ILogger<TvMazeApiService> _logger;
    public TvMazeApiService(ILogger<TvMazeApiService> logger)
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri("https://api.tvmaze.com/");
        _logger = logger;
    }

    private async Task<HttpResponseMessage> GetAsync(string url)
    {
        var response = await _client.GetAsync(url);
        _logger.LogInformation("The response was {Code} to request {Url}", response.StatusCode, url);
        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            _logger.LogError("Too Many Requests");
            Task.Delay(TimeSpan.FromSeconds(2)).Wait();
            return await GetAsync(url);
        }
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Didn't return proper value from url {Url} with code {Code}", url, response.StatusCode);
        }
        return response;
    }

    private ShowRoot DeserializeShow(long id, string content)
    {
        try
        {
            return JsonConvert.DeserializeObject<ShowRoot>(content);
        }
        catch
        {
            _logger.LogError("Problem with id {Id}", id);
            throw;
        }
    }

    public async Task<ShowRoot> getShow(long id)
    {
        var result = await GetAsync($"shows/{id}?embed=cast");
        return DeserializeShow(id, await result.Content.ReadAsStringAsync());
    }

    public async Task<Dictionary<long, long>> getShowUpdate()
    {
        var result = await GetAsync("updates/shows");
        return JsonConvert.DeserializeObject<Dictionary<long, long>>(await result.Content.ReadAsStringAsync());
    }
}