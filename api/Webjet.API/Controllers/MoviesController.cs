using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Webjet.Models;
using Webjet.Services;

namespace Webjet.Controllers;

[ApiController]
[Route("[controller]")]
public class MoviesController : ControllerBase
{
    private readonly ICinemaWorldService _cinemaWorldService;
    private readonly IFilmWorldService _filmWorldService;
    private readonly ILogger<MoviesController> _logger;
    private readonly IMemoryCache _cache;

    public MoviesController(
        ICinemaWorldService cinemaWorldService,
        IFilmWorldService filmWorldService,
        ILogger<MoviesController> logger,
        IMemoryCache cache)
    {
        _cinemaWorldService = cinemaWorldService;
        _filmWorldService = filmWorldService;
        _logger = logger;
        _cache = cache;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAggregatedMovies()
    {
        try
        {
            // get tasks for both services
            var cwTask = _cinemaWorldService.GetMovies();
            var fwTask = _filmWorldService.GetMovies();

            // Run both tasks concurrently
            // Any exceptions will be caught inside the service and returns an empty list
            // so we can aggregate results without failing the entire requestß
            await Task.WhenAll(cwTask, fwTask);

            var movies = new List<Movie>();
            movies.AddRange(cwTask.Result);
            movies.AddRange(fwTask.Result);

            // idealy, I would move this mapping logic to a separate service or mapper class
            var dto = ConvertToDTO(movies);

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while aggregating movies.");
            return StatusCode(500, new { Error = "An unexpected error occurred.", Details = ex.Message });
        }
    }

    private List<MovieDTO> ConvertToDTO(List<Movie> movies)
    {
        // Group by CommonID and aggregate prices
        var grouped = movies
        .GroupBy(m => m.CommonID)
        .Select(g =>
        {
            // Find the cheapest price
            var minPrice = g.Select(m => new { m.Price }).Min(p => p.Price);

            var priceList = g.Select(p => new MoviePriceDTO
            {
                Source = p.Source,
                Price = p.Price,
                ID = p.ID,
                IsCheapest = p.Price == minPrice
            })
            .Where(p => p.Price.HasValue)
            .OrderByDescending(p => p.IsCheapest)
            .ToList();

            if (priceList.Count == 1)
            {
                priceList.First().IsCheapest = false;
            }
            
            // Return a new object with movie info and prices
            var movie = g.First();
            return new MovieDTO
            {
                CommonID = movie.CommonID,
                Title = movie.Title,
                Poster = movie.Poster,
                LowestPrice = minPrice,
                Prices = priceList,
            };
        }).ToList();
        return grouped;
    }

    [HttpGet("{movieId}")]
    public async Task<IActionResult> GetMovieDetails(string movieId)
    {
        try
        {
            MovieDetail? movieDetail = null;
            if (movieId.StartsWith("cw"))
                movieDetail = await _cinemaWorldService.GetMovieDetails(movieId);
            else if (movieId.StartsWith("fw"))
                movieDetail = await _filmWorldService.GetMovieDetails(movieId);

            if (movieDetail != null)
            {
                return Ok(movieDetail);
            }

            _logger.LogWarning("Movie with ID {MovieId} not found.", movieId);
            return NotFound($"Movie with ID '{movieId}' not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching movie details for {MovieId}", movieId);
            return StatusCode(500, new { Error = "Error fetching movie details.", Details = ex.Message });
        }
    }

    [HttpGet("cache-dump")]
    public IActionResult GetAllCacheData()
    {
        _cache.TryGetValue($"movies-Cinemaworld", out List<Movie>? cw);
        _cache.TryGetValue($"movies-Filmword", out List<Movie>? fw);

        var cacheEntries = new
        {
            Cinemaworld = cw ?? new List<Movie>(),
            Filmword = fw ?? new List<Movie>()
        };
        return Ok(cacheEntries);
    }
    [HttpDelete("clear-cache")]
    public IActionResult ClearCache()
    {
        _cache.Remove($"movies-Cinemaworld");
        _cache.Remove($"movies-Filmword");
        _logger.LogInformation("Cache cleared successfully.");
        return Ok("Cache cleared successfully.");
    }
}
