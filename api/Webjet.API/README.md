# Webjet.API

## Overview

Webjet.API is an ASP.NET Core Web API that provides endpoints for retrieving movie information from external sources (Cinemaworld and Filmworld). The API uses Polly for resilience.

---

## Building the Project

1. **Prerequisites**
   - [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
   - (Optional) Visual Studio 2022+ or VS Code

2. **Clone the Repository**
   ```sh
   git clone <your-repo-url>
   cd api/Webjet.API
   ```

3. **Restore Dependencies**
   ```sh
   dotnet restore
   ```

4. **Build the Project**
   ```sh
   dotnet build
   ```

---

## Running the API

You can run the API using the .NET CLI:

```sh
dotnet run
```

By default, the API will be available at:
- HTTP: `http://localhost:8080`
- HTTPS: `https://localhost:7279`

---

## Configuration: `appsettings.json`

The API uses `appsettings.json` for configuration. Here are the key variables:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ServiceUrls": {
    "AccessToken": "sjd1HfkjU83ksdsm3802k",
    "Cinemaworld": "https://webjetapitest.azurewebsites.net/api/cinemaworld/",
    "Filmword": "https://webjetapitest.azurewebsites.net/api/filmworld/"
  },
  "AllowedHosts": "*",
  "CacheExpirationMinutes": 30,
  "RequestTimeout": 15
}
```

### Variable Explanations

- **Logging**: Controls the logging level for the application and ASP.NET Core.
- **ServiceUrls**
  - `AccessToken`: The API token required for authenticating requests to Cinemaworld and Filmworld services.
  - `Cinemaworld`: Base URL for the Cinemaworld external API.
  - `Filmword`: Base URL for the Filmworld external API.
- **AllowedHosts**: Specifies which hosts are allowed to access the API. `"*"` allows all hosts.
- **CacheExpirationMinutes**: Duration (in minutes) for which movie data is cached in memory. Increasing this value reduces external API calls but may serve stale data.
- **RequestTimeout**: Timeout (in seconds) for outgoing HTTP requests to external APIs. If a request takes longer, it will be aborted and retried according to Polly policies.

### How Settings Affect API Functionality

- **ServiceUrls**: If these URLs or the access token are incorrect, the API will not be able to fetch movie data from external sources.
- **CacheExpirationMinutes**: Affects how fresh the movie data is. Lower values mean more frequent updates but more external calls.
- **RequestTimeout**: Affects how long the API waits for a response from external services before failing over or retrying.
- **Logging**: Controls the verbosity of logs for debugging and monitoring.

---

## License

MIT (or your chosen license)