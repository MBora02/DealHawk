using System.Collections.Generic;

namespace DealHawk.Domain.Entities
{

    public class StoreGame
    {

        public int Id { get; set; }

        public int GameId { get; set; }

        public int StoreId { get; set; }

        public string ExternalGameId { get; set; }

        public string DeepLinkUrl { get; set; }

        public Game Game { get; set; } 
        public Store Store { get; set; }

        public ICollection<CurrentPrice> CurrentPrices { get; set; } = new List<CurrentPrice>();
        public ICollection<PriceHistory> PriceHistories { get; set; } = new List<PriceHistory>();
    }
}
