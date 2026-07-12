using System.Collections.Generic;

namespace DealHawk.Domain.Entities
{

    public class Publisher
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Game> Games { get; set; }
    }
}
