using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Webjet.Models;

namespace Webjet.Services
{
    public interface ICinemaWorldService
    {
        Task<List<Movie>> GetMovies();
        Task<MovieDetail?> GetMovieDetails(string movieId);
    }

    public class CinemaWorldService : ICinemaWorldService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CinemaWorldService> _logger;
        private readonly IMemoryCache _cache;
        private readonly AppSettings _appSettings;
        private const string Source = "Cinemaworld";
        private const string IDPrefix = "cw";
        public CinemaWorldService(
            IHttpClientFactory httpClientFactory,
            ILogger<CinemaWorldService> logger,
            IMemoryCache cache,
            IOptions<AppSettings> appSettings)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _cache = cache;
            _appSettings = appSettings.Value;
        }

        public async Task<List<Movie>> GetMovies()
        {
            try
            {
                if (_appSettings.CachingEnabled && _cache.TryGetValue($"movies-{Source}", out List<Movie>? cachedMovies))
                {
                    _logger.LogInformation($"movies returned from cache");
                    return cachedMovies ?? new List<Movie>();
                }

                var client = _httpClientFactory.CreateClient(Source);
                var result = await client.GetAsync("movies");
                result.EnsureSuccessStatusCode();

                var response = await result.Content.ReadFromJsonAsync<MoviesResponse>();
                List<Movie> movieList = response?.Movies ?? new List<Movie>();

                List<Task<MovieDetail>> detailTasks = new List<Task<MovieDetail>>();
                movieList?.ForEach(x =>
                {
                    x.Source = Source;
                    x.CommonID = x.ID.Replace(IDPrefix, string.Empty);
                    // Fetch movie details concurrently
                    // not the best way to do this, but for test, we will fetch details in parallel
                    // Should consider using a more efficient way to fetch details, like batch requests if supported by the API
                    var detailTask = GetMovieDetails(x.ID);
                    if (detailTask != null) detailTasks.Add(detailTask!);
                });

                await Task.WhenAll(detailTasks);

                detailTasks.ForEach(task =>
                    {
                        if (task.Result != null)
                        {
                            var movie = movieList?.FirstOrDefault(m => m.ID == task.Result?.ID);
                            if (movie != null)
                            {
                                movie.Price = Convert.ToDecimal(task.Result?.Price);
                            }
                        }
                    });

                if (_appSettings.CachingEnabled)
                {
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(
                            _appSettings.CacheExpirationMinutes
                        ));

                    _cache.Set($"movies-{Source}", movieList, cacheOptions);
                }
                return movieList ?? new List<Movie>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Cinemaworld");
                return new List<Movie>();
            }
        }

        public async Task<MovieDetail?> GetMovieDetails(string movieId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(Source);
                var result = await client.GetAsync($"movie/{movieId}");
                result.EnsureSuccessStatusCode();

                var content = await result.Content.ReadAsStringAsync();
                var movieDetail = JsonSerializer.Deserialize<MovieDetail>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (movieDetail != null)
                {
                    movieDetail.Source = Source;
                    return movieDetail;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching movie details for {MovieId} from Cinemaworld.", movieId);
                return null;
            }
        }
    }
}
