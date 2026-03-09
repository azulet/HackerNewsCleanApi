using HackerNewsCleanApi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HackerNewsCleanApi.Controllers;

[EnableRateLimiting("hnLimiter")]
[ApiController]
[Route("api/[controller]")]
public class HackerNewsController : ControllerBase
{
    private readonly IHackerNewsService _service;

    public HackerNewsController(IHackerNewsService service)
    {
        _service = service;
    }

    [HttpGet("top")]
    public async Task<IActionResult> GetTopStories(int count = 10)
    {
        var stories = await _service.GetTopStoriesAsync(count);
        return Ok(stories);
    }
}