public class MovieDTO
{
    public string? CommonID { get; set; } = string.Empty;
    public string? Title { get; set; } = string.Empty;
    public string? Poster { get; set; } = string.Empty;
    public List<MoviePriceDTO> Prices { get; set; } = new();
    public decimal? LowestPrice { get; set; }
}

public class MoviePriceDTO
{
    public string? Source { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string? ID { get; set; } = string.Empty;
    public bool IsCheapest { get; set; }
}