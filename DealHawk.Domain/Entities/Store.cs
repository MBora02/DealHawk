using System.Collections.Generic;

namespace DealHawk.Domain.Entities
{

    public class Store
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string CheapSharkStoreId { get; set; }

        public bool IsActive { get; set; }

        public string LogoUrl { get; set; }

        public ICollection<StoreGame> StoreGames { get; set; } = new List<StoreGame>();
    }
}
