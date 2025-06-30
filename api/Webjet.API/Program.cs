using Polly;
using Polly.Extensions.Http;
using System.Text.Json;
using Webjet.Models;
using Webjet.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure console logging at the most verbose level. Local development only.
// In production, you would typically use a more robust logging solutions.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Trace);

builder.Services.AddLogging();
builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddMemoryCache();

// Register services
builder.Services.AddTransient<ICinemaWorldService, CinemaWorldService>();
builder.Services.AddTransient<IFilmWorldService, FilmWorldService>();

// Add CORS to allow all. This is for development purposes only.
// In production, you should restrict this to specific origins.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Bind AppSettings from configuration
builder.Services.Configure<AppSettings>(builder.Configuration);
var appSettings = builder.Configuration.Get<AppSettings>();
if (appSettings == null)
{
    throw new InvalidOperationException("AppSettings configuration is missing or invalid.");
}

// Setup Cinemaworld HttpClient with Polly policies
builder.Services.AddHttpClient("Cinemaworld", (sp, client) =>
{
    client.BaseAddress = new Uri(appSettings.ServiceUrls.Cinemaworld!);
    client.DefaultRequestHeaders.Add("x-access-token", appSettings.ServiceUrls.AccessToken);
    client.Timeout = TimeSpan.FromSeconds(appSettings.RequestTimeout);
})
.AddPolicyHandler(sp => PollyPolicies.GetRetryPolicy())
.AddPolicyHandler((sp, rp) => PollyPolicies.GetCircuitBreakerPolicy(sp.GetRequiredService<ILoggerFactory>().CreateLogger("Cinemaworld")))
.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(appSettings.RequestTimeout));

// Setup Filmword HttpClient with Polly policies
builder.Services.AddHttpClient("Filmword", (sp, client) =>
{
    client.BaseAddress = new Uri(appSettings.ServiceUrls.Filmword!);
    client.DefaultRequestHeaders.Add("x-access-token", appSettings.ServiceUrls.AccessToken);
    client.Timeout = TimeSpan.FromSeconds(appSettings.RequestTimeout);
})
.AddPolicyHandler(sp => PollyPolicies.GetRetryPolicy())
.AddPolicyHandler((sp, rp) => PollyPolicies.GetCircuitBreakerPolicy(sp.GetRequiredService<ILoggerFactory>().CreateLogger("Filmword")))
.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(appSettings.RequestTimeout));

var app = builder.Build();

// Use CORS
app.UseCors("AllowAll");
app.UseHealthChecks("/health");
app.MapControllers();
app.Run();
