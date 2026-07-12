using System;

namespace DealHawk.Domain.Entities
{

    public class PriceHistory
    {

        public int Id { get; set; }

        public int StoreGameId { get; set; }

        public decimal Price { get; set; }

        public DateTime RecordedAt { get; set; }

        public StoreGame StoreGame { get; set; } = new StoreGame();
    }
}
