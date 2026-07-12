using System;

namespace DealHawk.Domain.Entities
{

    public class CurrentPrice
    {

        public int Id { get; set; }

        public int StoreGameId { get; set; }

        public decimal Price { get; set; }

        public decimal RetailPrice { get; set; }

        public decimal SavingsPercent { get; set; }

        public DateTime LastUpdated { get; set; }

        public StoreGame StoreGame { get; set; } = new StoreGame();
    }
}
