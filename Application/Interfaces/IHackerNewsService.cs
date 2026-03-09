using HackerNewsCleanApi.Domain.Models;

namespace HackerNewsCleanApi.Application.Interfaces;

public interface IHackerNewsService
{
    Task<IEnumerable<HackerNewsItem>> GetTopStoriesAsync(int count);
}