using System.Text.Json;
using HackerNewsCleanApi.Domain.Models;

namespace HackerNewsCleanApi.Infrastructure.Clients;

public class HackerNewsClient
{
    private readonly HttpClient _httpClient;

    public HackerNewsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<int>> GetTopStoryIdsAsync()
    {
        var json = await _httpClient.GetStringAsync("topstories.json");
        return JsonSerializer.Deserialize<List<int>>(json)!;
    }

    public async Task<HackerNewsItem?> GetItemAsync(int id)
    {
        var json = await _httpClient.GetStringAsync($"item/{id}.json");
        return JsonSerializer.Deserialize<HackerNewsItem>(json);
    }
}