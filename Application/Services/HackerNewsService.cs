
using HackerNewsCleanApi.Application.Interfaces;
using HackerNewsCleanApi.Domain.Models;
using HackerNewsCleanApi.Infrastructure.Clients;
using Microsoft.Extensions.Caching.Memory;

public class HackerNewsService : IHackerNewsService
{
    private readonly HackerNewsClient _client;
    private readonly IMemoryCache _cache;

    public HackerNewsService(HackerNewsClient client, IMemoryCache cache)
    {
        _client = client;
        _cache = cache;
    }

    public async Task<IEnumerable<HackerNewsItem>> GetTopStoriesAsync(int count)
    {
        return await _cache.GetOrCreateAsync("topstories", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);

            var ids = await _client.GetTopStoryIdsAsync();

            var tasks = ids.Take(count)
                           .Select(id => _client.GetItemAsync(id));

            var stories = await Task.WhenAll(tasks);

            return stories
                .Where(s => s.Type == "story")
                .OrderByDescending(s => s.Score)
                .ToList();
        });
    }
}