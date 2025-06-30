namespace Webjet.Models
{
    public class AppSettings
    {
        public ServiceUrls ServiceUrls { get; set; } = new ServiceUrls();
        public string AllowedHosts { get; set; } = "*";
        public int CacheExpirationMinutes { get; set; } = 30;
        public int RequestTimeout { get; set; } = 10;
        public bool CachingEnabled { get; set; } = true;
    }

    public class ServiceUrls
    {
        public string AccessToken { get; set; } = string.Empty;
        public string Cinemaworld { get; set; } = string.Empty;
        public string Filmword { get; set; } = string.Empty;
    }
}
