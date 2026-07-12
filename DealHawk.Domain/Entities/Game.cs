using System;
using System.Collections.Generic;

namespace DealHawk.Domain.Entities
{

    public class Game
    {

        public int Id { get; set; }

        public string Title { get; set; }

        public string CheapSharkGameId { get; set; }

        public string? SteamAppId { get; set; }

        public string ThumbnailUrl { get; set; }

        public int? PublisherId { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public Publisher? Publisher { get; set; }

        public ICollection<GameGenre> GameGenres { get; set; } = new List<GameGenre>();
        public ICollection<GamePlatform> GamePlatforms { get; set; } = new List<GamePlatform>();
        public ICollection<StoreGame> StoreGames { get; set; } = new List<StoreGame>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<PriceAlert> PriceAlerts { get; set; } = new List<PriceAlert>();
    }
}
