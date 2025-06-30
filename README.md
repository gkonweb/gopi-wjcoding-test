# WebJet Coding Test Project

## Project Structure

- `ui/` — React app named front-end
- `api/` — net8.0 app named Webjet.API
---

## Design Explanation

The backend is designed as a RESTful ASP.NET Core Web API that aggregates movie data from two external providers: Cinemaworld and Filmworld. Each provider is accessed via a dedicated service (`CinemaWorldService` and `FilmWorldService`), which uses `HttpClientFactory` for HTTP calls and Polly for resilience (retry, circuit breaker, and timeout policies). The API supports in-memory caching to reduce redundant external calls and improve response times. For testing purposes, caching is currently disabled (`CachingEnabled` is set to `false` in the configuration) so that every request retrieves fresh, real-time data from the external APIs. 

When a client requests the `/movies/list` endpoint, the backend concurrently fetches movie lists from both providers, then for each movie, it fetches detailed pricing information (by calling the detail endpoint for each movie in parallel). The results are aggregated and returned as a single response, showing all available prices for each movie.

### Areas for Improvement

- **Efficient Price Retrieval:**  
  Currently, the `/movies/list` endpoint fetches the movie list and then makes a separate HTTP request for each movie to retrieve its price. This can be slow and inefficient, especially as the number of movies grows.  
  **Improvement:** If the external APIs support batch requests for movie details/prices, the backend should leverage them to reduce the number of HTTP calls and improve performance.

- **Combine Movie Info and Price in a Single Call:**  
  At present, movie information and price are retrieved in separate steps (list, then detail for each).  
  **Improvement:** If the external APIs can be enhanced to return both movie metadata and price in a single response, the backend could fetch all required data in one call per provider, significantly reducing total response time and complexity.

- **Scalability:**  
  For higher scalability, consider using distributed caching (e.g., Redis) instead of in-memory cache, especially if deploying multiple backend instances.

- **Separation of Concerns:**  
  The aggregation and DTO mapping logic is currently in the controller due to time constraint.  
  **Improvement:** I would move this logic to a dedicated service or use a mapping library (like AutoMapper) for better maintainability and testability.



## Prerequisites

- Node.js (v20 or higher recommended)
- npm
- dotnet 8.0

---

## Running the Backend

1. Open a terminal and navigate to the backend directory:
    ```sh
    cd api
    ```
2. Install dependencies:
    ```sh
    dotnet restore
    ```
3. Start the backend server:
    ```sh
    dotnet run
    ```
4. The backend runs at:  **http://localhost:8080**

---

## Running the Frontend

1. Open a new terminal and navigate to the frontend directory:
    ```sh
    cd ui/front-end
    ```
2. Install dependencies:
    ```sh
    npm install
    ```
3. Start the frontend development server:
    ```sh
    npm start
    ```
4. By default, the frontend runs at:  **http://localhost:3000**

---

