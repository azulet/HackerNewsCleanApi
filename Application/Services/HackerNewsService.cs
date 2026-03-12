
using HackerNewsCleanApi.Application.Interfaces;
using HackerNewsCleanApi.Domain.Models;
using HackerNewsCleanApi.Infrastructure.Clients;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

public class HackerNewsService : IHackerNewsService
{
    private readonly HackerNewsClient _client;
    private readonly IDistributedCache _cache;

    public HackerNewsService(HackerNewsClient client, IDistributedCache cache)
    {
        _client = client;
        _cache = cache;
    }

    //public async Task<IEnumerable<HackerNewsItem>> GetTopStoriesAsync(int count)
    //{
    //    return await _cache.GetOrCreateAsync("topstories", async entry =>
    //    {
    //        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2); //revisa que tenga datos en cach, dura
    //                                                                         //dos minutos
    //        var ids = await _client.GetTopStoryIdsAsync();

    //        var tasks = ids.Take(count)
    //                       .Select(id => _client.GetItemAsync(id)); // si no hay datos en caché llama a la api y trae top 10 ids

    //        var stories = await Task.WhenAll(tasks); //hace 10 llamadas en paralelo buscando ids 
    //                                                 //https://hacker-news.firebaseio.com/v0/item/{id}.json
    //        return stories
    //            .Where(s => s.Type == "story")
    //            .OrderByDescending(s => s.Score)
    //            .ToList();
    //    });
    //}

    public async Task<IEnumerable<HackerNewsItem>> GetTopStoriesAsync(int count)
    {
        var cacheKey = "topstories";

        var cachedData = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<List<HackerNewsItem>>(cachedData);
        }

        var ids = await _client.GetTopStoryIdsAsync();

        var tasks = ids.Take(count)
                       .Select(id => _client.GetItemAsync(id));

        var stories = await Task.WhenAll(tasks);

        var result = stories
            .Where(s => s != null && s.Type == "story")
            .OrderByDescending(s => s.Score)
            .ToList();

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
        };

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(result),
            options
        );

        return result;
    }


}