using System;
using System.Collections.Generic;

namespace DealHawk.Application.DTOs
{
    public class GameDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CheapSharkGameId { get; set; }
        public string? SteamAppId { get; set; }
        public string ThumbnailUrl { get; set; }
        public string CoverImageUrl => !string.IsNullOrEmpty(SteamAppId) && SteamAppId != "0" && !string.Equals(SteamAppId, "null", StringComparison.OrdinalIgnoreCase)
            ? $"https://cdn.akamai.steamstatic.com/steam/apps/{SteamAppId}/header.jpg"
            : (string.IsNullOrEmpty(ThumbnailUrl)
                ? "https://images.unsplash.com/photo-1542751371-adc38448a05e?auto=format&fit=crop&w=400&q=80"
                : ThumbnailUrl);
        public string? PublisherName { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public double AverageRating { get; set; }
        public List<string> Genres { get; set; }
        public List<string> Platforms { get; set; } = new List<string>();
        public List<CurrentPriceDto> CurrentPrices { get; set; }
    }
}
