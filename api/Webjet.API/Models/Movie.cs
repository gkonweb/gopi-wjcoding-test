namespace Webjet.Models;

public class Movie
{
    public string? Title { get; set; }
    public string? Year { get; set; }
    public required string ID { get; set; }
    public string? CommonID { get; set; }
    public string? Type { get; set; }
    public string? Poster { get; set; }
    public string? Source { get; set; }
    public decimal? Price { get; set; }
}
